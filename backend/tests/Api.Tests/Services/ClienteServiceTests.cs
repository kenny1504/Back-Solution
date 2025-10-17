using System.Linq.Expressions;
using System.Text;
using Application.DTO;
using Application.Services;
using AutoMapper;
using Domain.Abstractions;
using Domain.Entities;
using FluentValidation;
using Moq;

namespace Api.Tests.Services;

public class ClienteServiceTests
{
  private readonly Mock<IUnitOfWork> _uow;
        private readonly Mock<IRepository<Cliente>> _repoCliente;
        private readonly Mock<IMapper> _mapper;
        private readonly ClienteService _sut;

        public ClienteServiceTests()
        {
            _uow = new Mock<IUnitOfWork>(MockBehavior.Strict);
            _repoCliente = new Mock<IRepository<Cliente>>(MockBehavior.Strict);
            _mapper = new Mock<IMapper>(MockBehavior.Strict);
            _uow.Setup(x => x.Repository<Cliente>()).Returns(_repoCliente.Object);

            _uow.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);
            _sut = new ClienteService(_uow.Object, _mapper.Object);
        }

        [Fact]
        public async Task ListAsync_devuelve_lista_mapeada()
        {
            // Arrange
            var ct = CancellationToken.None;
            var entities = new List<Cliente>
            {
                new() { Id = 1, ClienteId = "cli1", Nombre = "Ana" },
                new() { Id = 2, ClienteId = "cli2", Nombre = "Juan" }
            };

            _repoCliente
                .Setup(r => r.ListAsync(null, ct))
                .ReturnsAsync(entities);

            var mapped = new List<ClienteDto>
            {
                new() { Id = 1, ClienteId = "cli1", Nombre = "Ana", Genero = "F", Edad = 30, Identificacion = "ID1", Direccion = "Dir1", Telefono = "123456789", Activo = true },
                new() { Id = 2, ClienteId = "cli2", Nombre = "Juan", Genero = "M", Edad = 40, Identificacion = "ID2", Direccion = "Dir2", Telefono = "987654321", Activo = false }
            };

            _mapper
                .Setup(m => m.Map<IEnumerable<ClienteDto>>(It.IsAny<IEnumerable<Cliente>>()))
                .Returns(mapped);

            // Act
            var result = await _sut.ListAsync(ct);

            // Assert
            Assert.Same(mapped, result);

            _repoCliente.Verify(r => r.ListAsync(null, ct), Times.Once);
            _mapper.Verify(m => m.Map<IEnumerable<ClienteDto>>(It.Is<IEnumerable<Cliente>>(l => l.Count() == 2)), Times.Once);
        }

        [Fact]
        public async Task GetAsync_devuelve_dto_mapeado_si_existe()
        {
            var ct = CancellationToken.None;
            var entity = new Cliente { Id = 10, ClienteId = "C10", Nombre = "Kenny" };

            _repoCliente.Setup(r => r.GetByIdAsync(10, ct)).ReturnsAsync(entity);

            var dto = new ClienteDto { Id = 10, ClienteId = "C10", Nombre = "Kenny" };
            _mapper.Setup(m => m.Map<ClienteDto>(entity)).Returns(dto);

            var result = await _sut.GetAsync(10, ct);

            Assert.NotNull(result);
            Assert.Same(dto, result);

            _repoCliente.Verify(r => r.GetByIdAsync(10, ct), Times.Once);
            _mapper.Verify(m => m.Map<ClienteDto>(entity), Times.Once);
        }

        [Fact]
        public async Task GetAsync_devuelve_null_si_no_existe()
        {
            var ct = CancellationToken.None;
            _repoCliente.Setup(r => r.GetByIdAsync(99, ct)).ReturnsAsync((Cliente?)null);

            var result = await _sut.GetAsync(99, ct);

            Assert.Null(result);
            _repoCliente.Verify(r => r.GetByIdAsync(99, ct), Times.Once);
            _mapper.Verify(m => m.Map<ClienteDto>(It.IsAny<Cliente>()), Times.Never);
        }



        [Fact]
        public async Task UpdateAsync_lanza_KeyNotFound_si_no_existe()
        {
            var ct = CancellationToken.None;
            _repoCliente.Setup(r => r.GetByIdAsync(7, ct)).ReturnsAsync((Cliente?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _sut.UpdateAsync(7, new ClienteUpdateDto(), ct)
            );

            _repoCliente.Verify(r => r.GetByIdAsync(7, ct), Times.Once);
            _uow.Verify(u => u.SaveChangesAsync(ct), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_lanza_Validation_si_ClienteId_duplicado()
        {
            var ct = CancellationToken.None;
            var existente = new Cliente
            {
                Id = 5,
                ClienteId = "old",
                Identificacion = "ID-OLD",
                Telefono = "555",
                Direccion = "Dir",
                Nombre = "Nombre",
                Genero = "M",
                Activo = true,
                ContrasenaHash = Convert.ToBase64String(Encoding.UTF8.GetBytes("x"))
            };

            _repoCliente.Setup(r => r.GetByIdAsync(5, ct)).ReturnsAsync(existente);

            _repoCliente
                .Setup(r => r.ListAsync(It.IsAny<Expression<Func<Cliente, bool>>>(), ct))
                .ReturnsAsync(new List<Cliente> { new Cliente { Id = 99, ClienteId = "nuevo" } });

            var dto = new ClienteUpdateDto
            {
                ClienteId = "nuevo",
                Identificacion = "ID-OLD",
                Nombre = "X",
                Telefono = "Y",
                Direccion = "Z",
                Activo = true,
                Genero = "M",
                Contrasena = ""
            };

            await Assert.ThrowsAsync<ValidationException>(() => _sut.UpdateAsync(5, dto, ct));

            _repoCliente.Verify(r => r.GetByIdAsync(5, ct), Times.Once);
            _repoCliente.Verify(r => r.ListAsync(It.IsAny<Expression<Func<Cliente, bool>>>(), ct), Times.Once);
            _uow.Verify(u => u.SaveChangesAsync(ct), Times.Never);
        }


        [Fact]
        public async Task UpdateAsync_actualiza_campos_y_hash_cuando_contrasena_no_vacia()
        {
            var ct = CancellationToken.None;
            var existente = new Cliente
            {
                Id = 5,
                ClienteId = "old",
                Identificacion = "OLD",
                Telefono = "555",
                Direccion = "Dir",
                Nombre = "Nombre",
                Genero = "M",
                Activo = true,
                ContrasenaHash = Convert.ToBase64String(Encoding.UTF8.GetBytes("Vieja"))
            };

            _repoCliente.Setup(r => r.GetByIdAsync(5, ct)).ReturnsAsync(existente);

            _repoCliente
                .Setup(r => r.ListAsync(It.IsAny<Expression<Func<Cliente, bool>>>(), ct))
                .ReturnsAsync(new List<Cliente>());

            var dto = new ClienteUpdateDto
            {
                ClienteId = "  NEWid  ",
                Identificacion = "  NEWDOC  ",
                Nombre = "  Ana  ",
                Telefono = "  777-999  ",
                Direccion = "  Calle X  ",
                Activo = false,
                Genero = "F",
                Contrasena = "NuevaClave"
            };

            await _sut.UpdateAsync(5, dto, ct);

            Assert.Equal("NEWid", existente.ClienteId);
            Assert.Equal("NEWDOC", existente.Identificacion);
            Assert.Equal("Ana", existente.Nombre);
            Assert.Equal("777-999", existente.Telefono);
            Assert.Equal("Calle X", existente.Direccion);
            Assert.False(existente.Activo);
            Assert.Equal("F", existente.Genero);

            var esperado = Convert.ToBase64String(Encoding.UTF8.GetBytes("NuevaClave"));
            Assert.Equal(esperado, existente.ContrasenaHash);

            _uow.Verify(u => u.SaveChangesAsync(ct), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_actualiza_campos_y_no_toca_hash_cuando_contrasena_vacia()
        {
            var ct = CancellationToken.None;
            var existente = new Cliente
            {
                Id = 5,
                ClienteId = "old",
                Identificacion = "OLD",
                Telefono = "555",
                Direccion = "Dir",
                Nombre = "Nombre",
                Genero = "M",
                Activo = true,
                ContrasenaHash = Convert.ToBase64String("Vieja"u8.ToArray())
            };

            _repoCliente.Setup(r => r.GetByIdAsync(5, ct)).ReturnsAsync(existente);

            _repoCliente
                .Setup(r => r.ListAsync(It.IsAny<Expression<Func<Cliente, bool>>>(), ct))
                .ReturnsAsync(new List<Cliente>());

            var dto = new ClienteUpdateDto
            {
                ClienteId = " NEW ",
                Identificacion = " DOC ",
                Nombre = "  Ana  ",
                Telefono = "   ",
                Direccion = "  Calle Y  ",
                Activo = true,
                Genero = "F",
                Contrasena = "   "
            };

            var hashOriginal = existente.ContrasenaHash;

            await _sut.UpdateAsync(5, dto, ct);

            Assert.Equal("NEW", existente.ClienteId);
            Assert.Equal("DOC", existente.Identificacion);
            Assert.Equal("Ana", existente.Nombre);
            Assert.Null(existente.Telefono);
            Assert.Equal("Calle Y", existente.Direccion);
            Assert.True(existente.Activo);
            Assert.Equal("F", existente.Genero);
            Assert.Equal(hashOriginal, existente.ContrasenaHash);

            _uow.Verify(u => u.SaveChangesAsync(ct), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_elimina_si_existe()
        {
            var ct = CancellationToken.None;
            var existente = new Cliente { Id = 77 };

            _repoCliente.Setup(r => r.GetByIdAsync(77, ct)).ReturnsAsync(existente);
            _repoCliente.Setup(r => r.Remove(existente));

            await _sut.DeleteAsync(77, ct);

            _repoCliente.Verify(r => r.GetByIdAsync(77, ct), Times.Once);
            _repoCliente.Verify(r => r.Remove(It.Is<Cliente>(c => c == existente)), Times.Once);
            _uow.Verify(u => u.SaveChangesAsync(ct), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_lanza_KeyNotFound_si_no_existe()
        {
            var ct = CancellationToken.None;
            _repoCliente.Setup(r => r.GetByIdAsync(77, ct)).ReturnsAsync((Cliente?)null);

            var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.DeleteAsync(77, ct));
            Assert.Equal("Cliente no encontrado", ex.Message);

            _repoCliente.Verify(r => r.GetByIdAsync(77, ct), Times.Once);
            _repoCliente.Verify(r => r.Remove(It.IsAny<Cliente>()), Times.Never);
            _uow.Verify(u => u.SaveChangesAsync(ct), Times.Never);
        }
}
