namespace Application.DTO.Movimiento;

public class EstadoCuentaCuentaDto
{
  public int CuentaId { get; set; }
  public string NumeroCuenta { get; set; } = default!;
  public string TipoCuenta { get; set; } = default!;
  public decimal SaldoActual { get; set; }
  public decimal TotalCreditos { get; set; }
  public decimal TotalDebitos { get; set; }
  public List<MovimientoListItemDto> Movimientos { get; set; } = new();
}
