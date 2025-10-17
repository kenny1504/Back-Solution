namespace Application.DTO;

public class CreateMovimientoDto
{
  public int CuentaId { get; set; }
  public int Tipo { get; set; }
  public decimal Valor { get; set; }
  public DateTime? Fecha { get; set; }
}
