using System.ComponentModel.DataAnnotations;
using Application.DTO;
using AutoMapper;
using Domain.Abstractions;
using Domain.Entities;

namespace Application.Services;

public class CuentaService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public CuentaService(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CuentaListItemDto>> ListAsync(CancellationToken ct)
    {
        var list = (await _uow.Repository<Cuenta>().ListAsync(c => true, ct))
            .Select(c => new CuentaListItemDto
            {
                Id = c.Id,
                NumeroCuenta = c.Numero,
                TipoCuenta = c.Tipo.ToString(),
                SaldoInicial = c.SaldoInicial,
                Activa = c.Activa,
                Cliente = c.Cliente.Nombre
            })
            .OrderBy(x => x.NumeroCuenta)
            .ToList();

        return list;
    }

    public async Task<CuentaDto?> GetAsync(int id, CancellationToken ct)
    {
        var cuenta = await _uow.Repository<Cuenta>().GetByIdAsync(id, ct);
        return cuenta is null ? null : _mapper.Map<CuentaDto>(cuenta);
    }

    public async Task<int> CreateAsync(CreateCuentaDto dto, CancellationToken ct)
    {

        var clienteExists = await _uow.Repository<Cliente>()
            .ListAsync(c => c.Id == dto.ClienteId, ct)
            .ContinueWith(t => t.Result.Any(), ct);
        if (!clienteExists) throw new ValidationException("Cliente no existe.");


        var numeroDuplicado = await _uow.Repository<Cuenta>()
            .ListAsync(c => c.Numero == dto.Numero, ct)
            .ContinueWith(t => t.Result.Any(), ct);
        if (numeroDuplicado) throw new ValidationException("Número de cuenta ya existe.");

        var entity = new Cuenta
        {
            Numero = dto.Numero.Trim(),
            Tipo = (TipoCuenta)dto.Tipo,
            SaldoInicial = dto.SaldoInicial,
            Activa = dto.Activa,
            ClienteIdFk = dto.ClienteId
        };

        await _uow.Repository<Cuenta>().AddAsync(entity, ct);
        await _uow.SaveChangesAsync(ct);
        return entity.Id;
    }

    public async Task UpdateAsync(int id, UpdateCuentaDto dto, CancellationToken ct)
    {
        var cuenta = await _uow.Repository<Cuenta>().GetByIdAsync(id, ct);
        if (cuenta is null) throw new KeyNotFoundException($"Cuenta {id} no existe.");

        if (!cuenta.Numero.Equals(dto.Numero, StringComparison.OrdinalIgnoreCase) &&
            await _uow.Repository<Cuenta>()
                .ListAsync(c => c.Numero == dto.Numero && c.Id != id, ct)
                .ContinueWith(t => t.Result.Any(), ct))
            throw new ValidationException("Número de cuenta ya existe.");

        var clienteExists = await _uow.Repository<Cliente>()
            .ListAsync(c => c.Id == dto.ClienteId, ct)
            .ContinueWith(t => t.Result.Any(), ct);
        if (!clienteExists) throw new ValidationException("Cliente no existe.");

        cuenta.Numero = dto.Numero.Trim();
        cuenta.Tipo = (TipoCuenta)dto.Tipo;
        cuenta.SaldoInicial = dto.SaldoInicial;
        cuenta.Activa = dto.Activa;
        cuenta.ClienteIdFk = dto.ClienteId;

        await _uow.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct)
    {
        var repo = _uow.Repository<Cuenta>();
        var cuenta = await repo.GetByIdAsync(id, ct) ?? throw new KeyNotFoundException("Cuenta no encontrada");
        repo.Remove(cuenta);
        await _uow.SaveChangesAsync(ct);
    }
}
