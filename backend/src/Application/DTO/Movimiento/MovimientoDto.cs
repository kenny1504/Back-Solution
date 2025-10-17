namespace Application.DTO;

public class MovimientoDto
{
  public int Id { get; set; }
  public int CuentaId { get; set; }
  public string Tipo { get; set; } = default!;
  public decimal Valor { get; set; }
  public decimal SaldoDisponible { get; set; }
  public DateTime Fecha { get; set; }
}
