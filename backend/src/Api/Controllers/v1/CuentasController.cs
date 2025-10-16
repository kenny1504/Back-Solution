using Application.DTO;
using Application.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public sealed class CuentasController : ControllerBase
{
    private readonly CuentaService _cuentaService;
    private readonly IValidator<CreateCuentaDto> _createV;
    private readonly IValidator<UpdateCuentaDto> _updateV;

    public CuentasController(CuentaService cuentaService,
        IValidator<CreateCuentaDto> createV,
        IValidator<UpdateCuentaDto> updateV)
    {
        _cuentaService = cuentaService;
        _createV = createV;
        _updateV = updateV;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CuentaListItemDto>>> GetAll(CancellationToken ct)
        => Ok(await _cuentaService.ListAsync(ct));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CuentaDto>> Get(int id, CancellationToken ct)
        => (await _cuentaService.GetAsync(id, ct)) is { } dto ? Ok(dto) : NotFound();

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCuentaDto dto, CancellationToken ct)
    {
        var validationResult = await _createV.ValidateAsync(dto, ct);
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
            await _cuentaService.CreateAsync(dto, ct);
            return Ok(new { mensaje = "Cuenta creado exitosamente" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { mensaje = "Ocurrió un error al crear Cuenta", detalle = ex.Message });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCuentaDto dto, CancellationToken ct)
    {
        var validationResult = await _updateV.ValidateAsync(dto, ct);
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
            await _cuentaService.UpdateAsync(id, dto, ct);
            return Ok(new { mensaje = "Cuenta actualizada exitosamente" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { mensaje = "Ocurrió un error al actualizar Cuenta", detalle = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        try
        {
            await _cuentaService.DeleteAsync(id, ct);
            return Ok(new { mensaje = "Cuenta eliminado exitosamente" });
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { mensaje = "cuenta no encontrado" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { mensaje = "Ocurrió un error al eliminar el cuenta", detalle = ex.Message });
        }
    }
}