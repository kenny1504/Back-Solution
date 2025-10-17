namespace Application.DTO;

public class ClienteDto
{
    public int Id { get; set; }
    public string ClienteId { get; set; } = null!;
    public string Nombre { get; set; } = null!;
    public string Genero { get; set; } = null!;
    public int Edad { get; set; }
    public string Identificacion { get; set; } = null!;
    public string Direccion { get; set; } = null!;
    public string Telefono { get; set; } = null!;
    public bool Activo { get; set; }
}
