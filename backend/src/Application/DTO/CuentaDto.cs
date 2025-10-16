namespace Application.DTO;

public sealed class CuentaDto
{
    public int Id { get; set; }
    public string NumeroCuenta { get; set; } = default!;
    public string TipoCuenta { get; set; } = default!;
    public decimal SaldoInicial { get; set; }
    public bool Estado { get; set; }
    public int ClienteId { get; set; }
    public byte[]? RowVersion { get; set; }
}