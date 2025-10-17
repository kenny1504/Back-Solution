using Application.DTO;
using Application.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class ClientesController : ControllerBase
{
  private readonly ClienteService _clienteService;
  private readonly IValidator<CreateClienteDto> _createClienteValidator;
  private readonly IValidator<ClienteUpdateDto> _updateClienteValidator;

  public ClientesController(ClienteService clienteService, IValidator<CreateClienteDto> createClienteValidator,
    IValidator<ClienteUpdateDto> updateClienteValidator)
  {
    _clienteService = clienteService;
    _createClienteValidator = createClienteValidator;
    _updateClienteValidator = updateClienteValidator;
  }

  [HttpGet]
  public async Task<IActionResult> GetAllClientes(CancellationToken cancellationToken)
  {
    var clientes = await _clienteService.ListAsync(cancellationToken);
    return Ok(clientes);
  }

  [HttpGet("{id:int}")]
  public async Task<IActionResult> GetClienteById(int id, CancellationToken cancellationToken)
  {
    var cliente = await _clienteService.GetAsync(id, cancellationToken);
    return cliente is null ? NotFound() : Ok(cliente);
  }

  [HttpPost]
  public async Task<IActionResult> CreateCliente([FromBody] CreateClienteDto createClienteDto,
    CancellationToken cancellationToken)
  {
    var validationResult = await _createClienteValidator.ValidateAsync(createClienteDto, cancellationToken);
    if (!validationResult.IsValid)
    {
      var validationErrors = validationResult.Errors
        .GroupBy(error => error.PropertyName)
        .ToDictionary(group => group.Key, group => group.Select(error => error.ErrorMessage).ToArray());

      return ValidationProblem(new ValidationProblemDetails(validationErrors)
      {
        Status = StatusCodes.Status400BadRequest,
        Title = "Error de Validación"
      });
    }


    try
    {
      await _clienteService.CreateAsync(createClienteDto, cancellationToken);
      return Ok(new { mensaje = "Cliente creado exitosamente" });
    }
    catch (Exception ex)
    {
      return StatusCode(500, new { mensaje = "Ocurrió un error al crear el cliente", detalle = ex.Message });
    }
  }

  [HttpPut("{id:int}")]
  public async Task<IActionResult> Update(int id, [FromBody] ClienteUpdateDto dto, CancellationToken ct)
  {
    var validationResult = await _updateClienteValidator.ValidateAsync(dto, ct);
    if (!validationResult.IsValid)
    {
      var validationErrors = validationResult.Errors
        .GroupBy(error => error.PropertyName)
        .ToDictionary(group => group.Key, group => group.Select(error => error.ErrorMessage).ToArray());

      return ValidationProblem(new ValidationProblemDetails(validationErrors)
      {
        Status = StatusCodes.Status400BadRequest,
        Title = "Error de Validación"
      });
    }

    try
    {
      await _clienteService.UpdateAsync(id, dto, ct);
      return Ok(new { mensaje = "Cliente actualizado exitosamente" });
    }
    catch (Exception ex)
    {
      return StatusCode(500, new { mensaje = "Ocurrió un error al actualizar el cliente", detalle = ex.Message });
    }
  }

  [HttpDelete("{id:int}")]
  public async Task<IActionResult> DeleteCliente(int id, CancellationToken cancellationToken)
  {
    try
    {
      await _clienteService.DeleteAsync(id, cancellationToken);
      return Ok(new { mensaje = "Cliente eliminado exitosamente" });
    }
    catch (KeyNotFoundException)
    {
      return NotFound(new { mensaje = "Cliente no encontrado" });
    }
    catch (Exception ex)
    {
      return StatusCode(500, new { mensaje = "Ocurrió un error al eliminar el cliente", detalle = ex.Message });
    }
  }
}
