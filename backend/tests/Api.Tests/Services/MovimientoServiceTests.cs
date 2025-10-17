using Application.DTO;
using Application.DTO.Movimiento;
using Application.Options;
using Application.Services;
using AutoMapper;
using Domain.Abstractions;
using Domain.Entities;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;

namespace Api.Tests.Services;

public class MovimientoServiceTests
{
  private readonly Mock<IUnitOfWork> _uow;
  private readonly Mock<IRepository<Movimiento>> _repoMov;
  private readonly Mock<IRepository<Cuenta>> _repoCta;
  private readonly Mock<IRepository<Cliente>> _repoCli;
  private readonly Mock<IMapper> _mapper;
  private readonly MovimientoService _sut;

  private readonly BankingOptions _opts;

  public MovimientoServiceTests()
  {
    _uow = new Mock<IUnitOfWork>(MockBehavior.Strict);
    _repoMov = new Mock<IRepository<Movimiento>>(MockBehavior.Strict);
    _repoCta = new Mock<IRepository<Cuenta>>(MockBehavior.Strict);
    _repoCli = new Mock<IRepository<Cliente>>(MockBehavior.Strict);
    _mapper = new Mock<IMapper>(MockBehavior.Strict);

    _opts = new BankingOptions { DailyDebitLimit = 500m };
    var opts = Options.Create(_opts);

    _uow.Setup(x => x.Repository<Movimiento>()).Returns(_repoMov.Object);
    _uow.Setup(x => x.Repository<Cuenta>()).Returns(_repoCta.Object);
    _uow.Setup(x => x.Repository<Cliente>()).Returns(_repoCli.Object);
    _uow.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

    _sut = new MovimientoService(_uow.Object, _mapper.Object, opts);
  }

  [Fact]
  public void Constructor_limita_DailyDebitLimit_a_1000()
  {
    var opts = Options.Create(new BankingOptions { DailyDebitLimit = 2000m });
    var sut = new MovimientoService(_uow.Object, _mapper.Object, opts);
    Assert.Equal(1000m, opts.Value.DailyDebitLimit);
  }

  [Fact]
  public async Task GetAsync_devuelve_dto_mapeado()
  {
    var ct = CancellationToken.None;
    var entity = new Movimiento { Id = 1, Valor = 200 };
    var dto = new MovimientoDto { Id = 1, Valor = 200 };

    _repoMov.Setup(r => r.GetByIdAsync(1, ct)).ReturnsAsync(entity);
    _mapper.Setup(m => m.Map<MovimientoDto>(entity)).Returns(dto);

    var result = await _sut.GetAsync(1, ct);

    Assert.NotNull(result);
    Assert.Equal(dto, result);

    _repoMov.Verify(r => r.GetByIdAsync(1, ct), Times.Once);
    _mapper.Verify(m => m.Map<MovimientoDto>(entity), Times.Once);
  }

  [Fact]
  public async Task CreateAsync_lanza_si_cuenta_no_existe()
  {
    var ct = CancellationToken.None;
    _repoCta.Setup(r => r.GetByIdAsync(99, ct)).ReturnsAsync((Cuenta?)null);

    var dto = new CreateMovimientoDto { CuentaId = 99, Tipo = 1, Valor = 100 };

    await Assert.ThrowsAsync<ValidationException>(() => _sut.CreateAsync(dto, ct));
  }


  [Fact]
  public async Task ReporteAsync_lanza_si_cliente_no_existe()
  {
    var ct = CancellationToken.None;
    _repoCli.Setup(r => r.GetByIdAsync(7, ct)).ReturnsAsync((Cliente?)null);

    var req = new EstadoCuentaRequest { ClienteId = 7, Desde = DateTime.UtcNow, Hasta = DateTime.UtcNow };
    await Assert.ThrowsAsync<ValidationException>(() => _sut.ReporteAsync(req, ct));
  }
}

internal static class MockDbSetExtensions
{
  public static Mock<DbSet<T>> BuildMockDbSet<T>(this IQueryable<T> data) where T : class
  {
    var mock = new Mock<DbSet<T>>();
    mock.As<IQueryable<T>>().Setup(m => m.Provider).Returns(data.Provider);
    mock.As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.Expression);
    mock.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(data.ElementType);
    mock.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
    return mock;
  }
}
