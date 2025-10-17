namespace Application.DTO;

public sealed class CuentaDto
{
    public int Id { get; set; }
    public string Numero { get; set; } = default!;
    public int Tipo { get; set; } = default!;
    public decimal SaldoInicial { get; set; }
    public bool Activa { get; set; }
    public int ClienteIdFk { get; set; }
}
