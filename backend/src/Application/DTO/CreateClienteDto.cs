namespace Application.DTO;

public class CreateClienteDto
{
    public string ClienteId { get; set; } = default!;
    public string Contrasena { get; set; } = default!;
    public string Nombre { get; set; } = default!;
    public string Genero { get; set; } = default!;
    public int Edad { get; set; }
    public string Identificacion { get; set; } = default!;
    public string Direccion { get; set; } = default!;
    public string Telefono { get; set; } = default!;
}
