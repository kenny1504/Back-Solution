namespace Application.DTO.Movimiento;

public class MovimientoListItemDto
{
  public int Id { get; set; }
  public DateTime Fecha { get; set; }
  public string Tipo { get; set; } = default!;
  public decimal SaldoInicial { get; set; }
  public decimal SaldoDisponible { get; set; }

  public string Estado { get; set; }

  public decimal Movimiento { get; set; }

  public string Numero { get; set; }

  public string Cliente { get; set; }
}
