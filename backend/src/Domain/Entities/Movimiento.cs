namespace Domain.Entities;

public class Movimiento
{
    public int Id { get; set; } // PK
    public DateTime Fecha { get; set; }
    public int Tipo { get; set; }
    public decimal Valor { get; set; }
    public decimal Saldo { get; set; }
    public int CuentaIdFk { get; set; }
    public Cuenta Cuenta { get; set; } = null!;
}
