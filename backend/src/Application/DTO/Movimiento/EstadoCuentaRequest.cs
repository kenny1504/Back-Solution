namespace Application.DTO.Movimiento;

public class EstadoCuentaRequest
{
  public int ClienteId { get; set; }
  public DateTime Desde { get; set; }
  public DateTime Hasta { get; set; }
  public string? Formato { get; set; }
}
