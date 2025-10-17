using System.Linq.Expressions;
using Application.DTO;
using Application.Services;
using AutoMapper;
using Domain.Abstractions;
using Domain.Entities;
using FluentValidation;
using Moq;

namespace Api.Tests.Services;

public class CuentaServiceTests
{
  private readonly Mock<IUnitOfWork> _uow;
  private readonly Mock<IRepository<Cuenta>> _repoCuenta;
  private readonly Mock<IRepository<Cliente>> _repoCliente;
  private readonly Mock<IMapper> _mapper;
  private readonly CuentaService _sut;

  public CuentaServiceTests()
  {
    _uow = new Mock<IUnitOfWork>(MockBehavior.Strict);
    _repoCuenta = new Mock<IRepository<Cuenta>>(MockBehavior.Strict);
    _repoCliente = new Mock<IRepository<Cliente>>(MockBehavior.Strict);
    _mapper = new Mock<IMapper>(MockBehavior.Strict);

    _uow.Setup(x => x.Repository<Cuenta>()).Returns(_repoCuenta.Object);
    _uow.Setup(x => x.Repository<Cliente>()).Returns(_repoCliente.Object);
    _uow.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

    _sut = new CuentaService(_uow.Object, _mapper.Object);
  }


  [Fact]
  public async Task GetAsync_devuelve_dto_mapeado_si_existe()
  {
    var ct = CancellationToken.None;
    var entity = new Cuenta
      { Id = 10, Numero = "ABC", Tipo = TipoCuenta.Ahorros, SaldoInicial = 100, Activa = true, ClienteIdFk = 1 };
    var dto = new CuentaDto { Id = 10, Numero = "ABC", Tipo = 1, SaldoInicial = 100, Activa = true, ClienteIdFk = 1 };

    _repoCuenta.Setup(r => r.GetByIdAsync(10, ct)).ReturnsAsync(entity);
    _mapper.Setup(m => m.Map<CuentaDto>(entity)).Returns(dto);

    var result = await _sut.GetAsync(10, ct);

    Assert.NotNull(result);
    Assert.Same(dto, result);

    _repoCuenta.Verify(r => r.GetByIdAsync(10, ct), Times.Once);
    _mapper.Verify(m => m.Map<CuentaDto>(entity), Times.Once);
  }

  [Fact]
  public async Task GetAsync_devuelve_null_si_no_existe()
  {
    var ct = CancellationToken.None;
    _repoCuenta.Setup(r => r.GetByIdAsync(99, ct)).ReturnsAsync((Cuenta?)null);

    var result = await _sut.GetAsync(99, ct);

    Assert.Null(result);
    _repoCuenta.Verify(r => r.GetByIdAsync(99, ct), Times.Once);
    _mapper.Verify(m => m.Map<CuentaDto>(It.IsAny<Cuenta>()), Times.Never);
  }


  [Fact]
  public async Task CreateAsync_agrega_y_guarda_retorna_Id()
  {
    var ct = CancellationToken.None;
    var dto = new CreateCuentaDto
    {
      Numero = "  001-ABC ",
      Tipo = 2,
      SaldoInicial = 75.5m,
      Activa = false,
      ClienteId = 3
    };

    _repoCliente.Setup_ListAsync_Predicate(new List<Cliente> { new Cliente { Id = 3 } });
    _repoCuenta.Setup_ListAsync_Predicate(new List<Cuenta>());

    _repoCuenta
      .Setup(r => r.AddAsync(It.IsAny<Cuenta>(), ct))
      .Callback<Cuenta, CancellationToken>((c, _) => c.Id = 42)
      .Returns(Task.CompletedTask);

    var id = await _sut.CreateAsync(dto, ct);

    Assert.Equal(42, id);
    _repoCuenta.Verify(r => r.AddAsync(It.Is<Cuenta>(c =>
      c.Numero == "001-ABC" &&
      c.Tipo == TipoCuenta.Corriente &&
      c.SaldoInicial == 75.5m &&
      c.Activa == false &&
      c.ClienteIdFk == 3
    ), ct), Times.Once);
    _uow.Verify(u => u.SaveChangesAsync(ct), Times.Once);
  }


  [Fact]
  public async Task UpdateAsync_lanza_KeyNotFound_si_no_existe()
  {
    var ct = CancellationToken.None;
    _repoCuenta.Setup(r => r.GetByIdAsync(7, ct)).ReturnsAsync((Cuenta?)null);

    await Assert.ThrowsAsync<KeyNotFoundException>(() =>
      _sut.UpdateAsync(7, new UpdateCuentaDto(), ct)
    );

    _repoCuenta.Verify(r => r.GetByIdAsync(7, ct), Times.Once);
    _uow.Verify(u => u.SaveChangesAsync(ct), Times.Never);
  }


  [Fact]
  public async Task UpdateAsync_actualiza_campos_y_guarda()
  {
    var ct = CancellationToken.None;
    var existente = new Cuenta
    {
      Id = 5,
      Numero = "OLD",
      Tipo = TipoCuenta.Ahorros,
      SaldoInicial = 10m,
      Activa = true,
      ClienteIdFk = 1
    };

    _repoCuenta.Setup(r => r.GetByIdAsync(5, ct)).ReturnsAsync(existente);

    _repoCuenta.Setup_ListAsync_Predicate(new List<Cuenta>());

    _repoCliente.Setup_ListAsync_Predicate(new List<Cliente> { new Cliente { Id = 3 } });

    var dto = new UpdateCuentaDto
    {
      Numero = "  NEWNUM  ",
      Tipo = 2,
      SaldoInicial = 250.75m,
      Activa = false,
      clienteIdFk = 3
    };

    await _sut.UpdateAsync(5, dto, ct);

    Assert.Equal("NEWNUM", existente.Numero);
    Assert.Equal(TipoCuenta.Corriente, existente.Tipo);
    Assert.Equal(250.75m, existente.SaldoInicial);
    Assert.False(existente.Activa);
    Assert.Equal(3, existente.ClienteIdFk);

    _uow.Verify(u => u.SaveChangesAsync(ct), Times.Once);
  }


  [Fact]
  public async Task DeleteAsync_lanza_si_no_existe()
  {
    var ct = CancellationToken.None;
    _repoCuenta.Setup(r => r.GetByIdAsync(77, ct)).ReturnsAsync((Cuenta?)null);

    var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.DeleteAsync(77, ct));
    Assert.Equal("Cuenta no encontrada", ex.Message);

    _repoCuenta.Verify(r => r.GetByIdAsync(77, ct), Times.Once);
    _repoCuenta.Verify(r => r.Remove(It.IsAny<Cuenta>()), Times.Never);
    _uow.Verify(u => u.SaveChangesAsync(ct), Times.Never);
  }

  [Fact]
  public async Task DeleteAsync_elimina_y_guarda()
  {
    var ct = CancellationToken.None;
    var cuenta = new Cuenta { Id = 77, Numero = "X" };

    _repoCuenta.Setup(r => r.GetByIdAsync(77, ct)).ReturnsAsync(cuenta);
    _repoCuenta.Setup(r => r.Remove(cuenta));

    await _sut.DeleteAsync(77, ct);

    _repoCuenta.Verify(r => r.Remove(It.Is<Cuenta>(c => c == cuenta)), Times.Once);
    _uow.Verify(u => u.SaveChangesAsync(ct), Times.Once);
  }
}

internal static class RepoListAsyncPredicateHelper
{
  public static void Setup_ListAsync_Predicate<T>(
    this Mock<IRepository<T>> repo,
    IEnumerable<T> data) where T : class
  {
    repo
      .Setup(r => r.ListAsync(It.IsAny<Expression<Func<T, bool>>>(), It.IsAny<CancellationToken>()))
      .ReturnsAsync((Expression<Func<T, bool>>? predicate, CancellationToken _) =>
      {
        var src = data ?? Enumerable.Empty<T>();
        if (predicate == null)
          return src.ToList().AsReadOnly();

        var compiled = predicate.Compile();
        return src.Where(compiled).ToList().AsReadOnly();
      });
  }
}
