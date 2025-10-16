namespace Domain.Entities;

public class Movimiento
{
    public int Id { get; set; } // PK
    public DateTime Fecha { get; set; }
    public string Tipo { get; set; } = null!;   // debito|credito
    public decimal Valor { get; set; }
    public decimal Saldo { get; set; }          // saldo después del mov.
    public int CuentaIdFk { get; set; }
    public Cuenta Cuenta { get; set; } = null!;
}
