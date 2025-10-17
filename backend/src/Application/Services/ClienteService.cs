using Application.DTO;
using AutoMapper;
using Domain.Abstractions;
using Domain.Entities;
using FluentValidation;

namespace Application.Services;

public class ClienteService
{
  private readonly IUnitOfWork _uow;
  private readonly IMapper _mapper;

  public ClienteService(IUnitOfWork uow, IMapper mapper)
  {
    _uow = uow;
    _mapper = mapper;
  }

  public async Task<IEnumerable<ClienteDto>> ListAsync(CancellationToken ct)
  {
    var list = await _uow.Repository<Cliente>().ListAsync(null, ct);
    return _mapper.Map<IEnumerable<ClienteDto>>(list);
  }

  public async Task<ClienteDto?> GetAsync(int id, CancellationToken ct)
  {
    var entity = await _uow.Repository<Cliente>().GetByIdAsync(id, ct);
    return entity is null ? null : _mapper.Map<ClienteDto>(entity);
  }

  public async Task<int> CreateAsync(CreateClienteDto dto, CancellationToken ct)
  {
    var hash = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(dto.Contrasena));

    var cliente = new Cliente
    {
      ClienteId = dto.ClienteId,
      ContrasenaHash = hash,
      Activo = true,
      Nombre = dto.Nombre,
      Genero = dto.Genero,
      Edad = dto.Edad,
      Identificacion = dto.Identificacion,
      Direccion = dto.Direccion,
      Telefono = dto.Telefono
    };

    await _uow.Repository<Cliente>().AddAsync(cliente, ct);
    await _uow.SaveChangesAsync(ct);
    return cliente.Id;
  }

  public async Task UpdateAsync(int id, ClienteUpdateDto dto, CancellationToken ct)
  {
    var cliente = await _uow.Repository<Cliente>().GetByIdAsync(id, ct);
    if (cliente is null) throw new KeyNotFoundException($"Cliente {id} no existe.");

    if (!cliente.ClienteId.Equals(dto.ClienteId, StringComparison.OrdinalIgnoreCase) &&
        await _uow.Repository<Cliente>().ListAsync(c => c.ClienteId == dto.ClienteId && c.Id != id, ct)
          .ContinueWith(t => t.Result.Any(), ct))
      throw new ValidationException("El ClienteId ya está registrado.");

    if (!cliente.Identificacion.Equals(dto.Identificacion, StringComparison.OrdinalIgnoreCase) &&
        await _uow.Repository<Cliente>().ListAsync(c => c.Identificacion == dto.Identificacion && c.Id != id, ct)
          .ContinueWith(t => t.Result.Any(), ct))
      throw new ValidationException("La identificación ya está registrada.");

    cliente.Nombre = dto.Nombre.Trim();
    cliente.ClienteId = dto.ClienteId.Trim();
    cliente.Telefono = string.IsNullOrWhiteSpace(dto.Telefono) ? null : dto.Telefono.Trim();
    cliente.Identificacion = dto.Identificacion.Trim();
    cliente.Direccion = dto.Direccion.Trim();
    cliente.Activo = dto.Activo;
    cliente.Genero = dto.Genero;

    if (!string.IsNullOrWhiteSpace(dto.Contrasena))
    {
      cliente.ContrasenaHash = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(dto.Contrasena));
    }

    await _uow.SaveChangesAsync(ct);
  }


  public async Task DeleteAsync(int id, CancellationToken ct)
  {
    var repo = _uow.Repository<Cliente>();
    var cliente = await repo.GetByIdAsync(id, ct) ?? throw new KeyNotFoundException("Cliente no encontrado");
    repo.Remove(cliente);
    await _uow.SaveChangesAsync(ct);
  }
}
