namespace Application.DTO;

public class CuentaListItemDto
{
    public int Id { get; set; }
    public string numero { get; set; } = default!;
    public string tipo { get; set; } = default!;
    public decimal saldo { get; set; }

    public bool Activa { get; set; }
    public string Cliente { get; set; } = default!;
}
