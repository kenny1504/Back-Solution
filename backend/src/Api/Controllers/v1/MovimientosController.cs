using Application.DTO;
using Application.DTO.Movimiento;
using Application.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public sealed class MovimientosController : ControllerBase
{
  private readonly MovimientoService _movimientoService;
  private readonly IValidator<CreateMovimientoDto> _createMovimientoValidator;

  public MovimientosController(MovimientoService svc, IValidator<CreateMovimientoDto> createV)
  {
    _movimientoService = svc;
    _createMovimientoValidator = createV;
  }


  [HttpGet]
  public async Task<ActionResult<IEnumerable<MovimientoListItemDto>>> ListByCuenta(CancellationToken ct)
    => Ok(await _movimientoService.ListByCuentaAsync(ct));

  [HttpGet("{id:int}")]
  public async Task<ActionResult<MovimientoDto>> Get(int id, CancellationToken ct)
    => (await _movimientoService.GetAsync(id, ct)) is { } dto ? Ok(dto) : NotFound();

  [HttpPost]
  public async Task<IActionResult> Create([FromBody] CreateMovimientoDto dto, CancellationToken ct)
  {
    var validationResult = await _createMovimientoValidator.ValidateAsync(dto, ct);
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
       await _movimientoService.CreateAsync(dto, ct);
      return Ok(new { mensaje = "Movimiento creado exitosamente" });
    }
    catch (Exception ex)
    {
      return StatusCode(500, new { mensaje = "Ocurrió un error al crear Movimiento", detalle = ex.Message });
    }
  }

  [HttpDelete("{id:int}")]
  public async Task<IActionResult> Delete(int id, CancellationToken ct)
  {

    try
    {
      await _movimientoService.DeleteAsync(id, ct);
      return Ok(new { mensaje = "Movimiento eliminado exitosamente" });
    }
    catch (KeyNotFoundException)
    {
      return NotFound(new { mensaje = "Movimiento no encontrado" });
    }
    catch (Exception ex)
    {
      return StatusCode(500, new { mensaje = "Ocurrió un error al eliminar el movimiento", detalle = ex.Message });
    }
  }

  [HttpGet("reporte")]
  public async Task<ActionResult<EstadoCuentaResult>> Reporte([FromQuery] EstadoCuentaRequest req, CancellationToken ct)
    => Ok(await _movimientoService.ReporteAsync(req, ct));
}
