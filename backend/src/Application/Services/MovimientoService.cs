using Application.DTO;
using Application.DTO.Movimiento;
using Application.Options;
using AutoMapper;
using Domain.Abstractions;
using Domain.Entities;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Application.Services;

public class MovimientoService
{
  private readonly IUnitOfWork _uow;
  private readonly IMapper _mapper;
  private readonly BankingOptions _opts;

  public MovimientoService(IUnitOfWork uow, IMapper mapper, IOptions<BankingOptions> opts)
  {
    _uow = uow;
    _mapper = mapper;
    _opts = opts.Value;
    if (_opts.DailyDebitLimit > 1000m) _opts.DailyDebitLimit = 1000m; // tope
  }

  public async Task<IEnumerable<MovimientoListItemDto>> ListByCuentaAsync(CancellationToken ct)
  {
    var q = _uow.Repository<Movimiento>().Query();
    return await q
      .OrderByDescending(m => m.Id)
      .Include(c => c.Cuenta)
      .Select(m => new MovimientoListItemDto
      {
        Id = m.Id,
        Fecha = m.Fecha,
        Tipo = m.Tipo == 1 ? "Ahorros" : "Corriente",
        SaldoInicial = m.Cuenta.SaldoInicial,
        SaldoDisponible = m.Saldo,
        Numero = m.Cuenta.Numero,
        Estado = m.Cuenta.Activa.ToString(),
        Movimiento = m.Valor,
        Cliente = m.Cuenta.Cliente.Nombre
      })
      .ToListAsync(ct);
  }

  public async Task<MovimientoDto?> GetAsync(int id, CancellationToken ct)
  {
    var mov = await _uow.Repository<Movimiento>().GetByIdAsync(id, ct);
    return mov is null ? null : _mapper.Map<MovimientoDto>(mov);
  }

  public async Task<int> CreateAsync(CreateMovimientoDto dto, CancellationToken ct)
  {
    var repoMov = _uow.Repository<Movimiento>();
    var repoCta = _uow.Repository<Cuenta>();

    var cuenta = await repoCta.GetByIdAsync(dto.CuentaId, ct)
                 ?? throw new ValidationException("La cuenta no existe.");

    var last = await repoMov.Query()
      .Where(m => m.CuentaIdFk == dto.CuentaId)
      .OrderByDescending(m => m.Id)
      .FirstOrDefaultAsync(ct);
    var saldoAnterior = last?.Saldo ?? cuenta.SaldoInicial;

    var esDebito = dto.Tipo == 2;
    var esCredito = dto.Tipo == 1;
    var monto = dto.Valor;

    if (esDebito)
    {
      if (saldoAnterior <= 0 || monto > saldoAnterior)
        throw new ValidationException("Saldo no disponible");

      var hoy = (dto.Fecha ?? DateTime.UtcNow).Date;
      var fin = hoy.AddDays(1);

      var sumDebitosHoy = await repoMov.Query()
        .Where(m => m.CuentaIdFk == dto.CuentaId
                    && m.Fecha >= hoy && m.Fecha < fin
                    && m.Valor < 0)
        .SumAsync(m => -m.Valor, ct);

      var limite = Math.Min(_opts.DailyDebitLimit, 1000m);
      if (monto > limite - sumDebitosHoy)
        throw new ValidationException("Cupo diario Excedido");
    }

    var valorFirmado = esCredito ? +monto : -monto;
    var nuevoSaldo = saldoAnterior + valorFirmado;

    var mov = new Movimiento
    {
      CuentaIdFk = dto.CuentaId,
      Tipo = dto.Tipo,
      Valor = valorFirmado,
      Saldo = nuevoSaldo,
      Fecha = dto.Fecha?.ToUniversalTime() ?? DateTime.UtcNow
    };

    await repoMov.AddAsync(mov, ct);
    await _uow.SaveChangesAsync(ct);
    return mov.Id;
  }

  public async Task DeleteAsync(int id, CancellationToken ct)
  {
    var repo = _uow.Repository<Movimiento>();
    var movimiento = await repo.GetByIdAsync(id, ct) ?? throw new KeyNotFoundException("Movimiento no encontrado");

    var ultimo = await repo.Query()
      .Where(m => m.CuentaIdFk == movimiento.CuentaIdFk)
      .OrderByDescending(m => m.Id)
      .FirstOrDefaultAsync(ct);

    if (ultimo?.Id != movimiento.Id)
      throw new ValidationException("Sólo se puede eliminar el último movimiento de la cuenta.");

    repo.Remove(movimiento);
    await _uow.SaveChangesAsync(ct);
  }


  public async Task<EstadoCuentaResult> ReporteAsync(EstadoCuentaRequest req, CancellationToken ct)
  {
    var repoCli = _uow.Repository<Cliente>();
    var repoCta = _uow.Repository<Cuenta>();
    var repoMov = _uow.Repository<Movimiento>();

    var cliente = await repoCli.GetByIdAsync(req.ClienteId, ct)
                  ?? throw new ValidationException("Cliente no existe.");

    var desde = req.Desde.ToUniversalTime().Date;
    var hasta = req.Hasta.ToUniversalTime().Date.AddDays(1).AddTicks(-1);

    var cuentas = await repoCta.Query()
      .Where(c => c.ClienteIdFk == req.ClienteId)
      .Select(c => new { c.Id, c.Numero, c.Tipo, c.SaldoInicial })
      .ToListAsync(ct);

    var respuesta = new EstadoCuentaResult
    {
      ClienteId = cliente.Id,
      Cliente = cliente.Nombre,
      Desde = desde,
      Hasta = hasta
    };

    foreach (var cta in cuentas)
    {
      var movs = await repoMov.Query()
        .Where(m => m.CuentaIdFk == cta.Id && m.Fecha >= desde && m.Fecha <= hasta)
        .OrderBy(m => m.Fecha)
        .Select(m => new MovimientoListItemDto
        {
          Fecha = m.Fecha,
          Tipo = m.Tipo == 1 ? "Credito" : "Debito",
          Movimiento = m.Valor,
          SaldoDisponible = m.Saldo
        })
        .ToListAsync(ct);

      var totalCred = movs.Where(m => m.Movimiento > 0).Sum(m => m.Movimiento);
      var totalDeb = movs.Where(m => m.Movimiento < 0).Sum(m => -m.Movimiento);

      var lastMov = await repoMov.Query()
        .Where(m => m.CuentaIdFk == cta.Id)
        .OrderByDescending(m => m.Fecha)
        .FirstOrDefaultAsync(ct);

      var saldoActual = lastMov?.Saldo ?? cta.SaldoInicial;

      respuesta.Cuentas.Add(new EstadoCuentaCuentaDto
      {
        CuentaId = cta.Id,
        NumeroCuenta = cta.Numero,
        TipoCuenta = cta.Tipo.ToString(),
        SaldoActual = saldoActual,
        TotalCreditos = totalCred,
        TotalDebitos = totalDeb,
        Movimientos = movs
      });
    }

    respuesta.PdfBase64 = await RenderPdfAsync(respuesta, ct);
    return respuesta;
  }

  private static Task<string> RenderPdfAsync(EstadoCuentaResult data, CancellationToken _)
  {
    QuestPDF.Settings.License = LicenseType.Community;
    QuestPDF.Settings.CheckIfAllTextGlyphsAreAvailable = false;
    QuestPDF.Settings.EnableDebugging = false;

    var doc = Document.Create(container =>
    {
      container.Page(page =>
      {
        page.Size(PageSizes.A4);
        page.Margin(25);

        page.DefaultTextStyle(x => x.FontSize(10));
        page.Header().PaddingBottom(8).Row(row =>
        {
          row.RelativeItem().Text(t => { t.Span("Estado de Cuenta").FontSize(18).SemiBold(); });

          row.ConstantItem(220).AlignRight().Column(col =>
          {
            col.Item().Text(t => t.Span($"Cliente: {data.Cliente}"));
            col.Item().Text(t => t.Span($"Rango: {data.Desde:yyyy-MM-dd} a {data.Hasta:yyyy-MM-dd}"));
          });
        });

        page.Content().Column(col =>
        {
          foreach (var cta in data.Cuentas)
          {
            col.Item().PaddingTop(10)
              .Text(t => t.Span($"Cuenta {cta.NumeroCuenta} · {cta.TipoCuenta}").SemiBold());

            col.Item().Row(r =>
            {
              r.RelativeItem().Text(t => t.Span($"Saldo actual: {cta.SaldoActual:N2}"));
              r.RelativeItem().Text(t => t.Span($"Créditos: {cta.TotalCreditos:N2}"));
              r.RelativeItem().Text(t => t.Span($"Débitos: {cta.TotalDebitos:N2}"));
            });

            col.Item().Table(t =>
            {
              t.ColumnsDefinition(cols =>
              {
                cols.ConstantColumn(110); // Fecha
                cols.RelativeColumn(); // Tipo
                cols.ConstantColumn(90); // Valor
                cols.ConstantColumn(90); // Saldo
              });

              t.Header(h =>
              {
                h.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2)
                  .Text(tt => tt.Span("Fecha").SemiBold());

                h.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2)
                  .Text(tt => tt.Span("Tipo").SemiBold());

                h.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2)
                  .Text(tt => tt.Span("Valor").SemiBold());

                h.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2)
                  .Text(tt => tt.Span("Saldo").SemiBold());
              });

              foreach (var m in cta.Movimientos)
              {
                t.Cell().Text(tt => tt.Span(m.Fecha.ToString("yyyy-MM-dd HH:mm")));
                t.Cell().Text(tt => tt.Span(m.Tipo));
                t.Cell().Text(tt => tt.Span(m.Movimiento.ToString("N2")));
                t.Cell().Text(tt => tt.Span(m.SaldoDisponible.ToString("N2")));
              }
            });
          }
        });

        page.Footer().AlignRight().Text(t =>
        {
          t.Span("Generado: ").Light();
          t.Span($"{DateTime.UtcNow:yyyy-MM-dd HH:mm} UTC");
        });
      });
    });

    var bytes = doc.GeneratePdf();
    return Task.FromResult(Convert.ToBase64String(bytes));
  }
}
