namespace Application.DTO;

public class CuentaListItemDto
{
    public int Id { get; set; }
    public string NumeroCuenta { get; set; } = default!;
    public string TipoCuenta { get; set; } = default!;
    public decimal SaldoInicial { get; set; }
    
    public bool Activa { get; set; }
    public string Cliente { get; set; } = default!;
}