namespace Domain.Entities;

public class Persona
{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public string Genero { get; set; } = null!;
    public int Edad { get; set; }
    public string Identificacion { get; set; } = null!;
    public string Direccion { get; set; } = null!;
    public string Telefono { get; set; } = null!;
}
