namespace Application.DTO.Movimiento;

public class EstadoCuentaResult
{
  public int ClienteId { get; set; }
  public string Cliente { get; set; } = default!;
  public DateTime Desde { get; set; }
  public DateTime Hasta { get; set; }
  public List<EstadoCuentaCuentaDto> Cuentas { get; set; } = new();
  public string? PdfBase64 { get; set; }
}
