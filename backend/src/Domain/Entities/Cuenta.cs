namespace Domain.Entities;
public enum TipoCuenta { Ahorros = 1, Corriente = 2 }

public class Cuenta
{
    public int Id { get; set; } // PK
    public string Numero { get; set; } = null!;
    public TipoCuenta Tipo { get; set; }
    public decimal SaldoInicial { get; set; }
    public bool Activa { get; set; }               // estado
    public int ClienteIdFk { get; set; }
    public Cliente Cliente { get; set; } = null!;
    public ICollection<Movimiento> Movimientos { get; set; } = new List<Movimiento>();
}