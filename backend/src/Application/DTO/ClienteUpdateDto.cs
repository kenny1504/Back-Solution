using System.ComponentModel.DataAnnotations;

namespace Application.DTO;

public class ClienteUpdateDto
{
    [Required] public string Nombre { get; set; } = default!;
    
    [Required] public string ClienteId { get; set; } = default!;
    [Phone][Required]   public string? Telefono { get; set; }
    [Required] public string Identificacion { get; set; } = default!;
    [Required] public string Direccion { get; set; } = default!;
    [Required] public bool Activo { get; set; }
    
    [Required] public int Edad { get; set; }
    
    [Required] public string Genero { get; set; }
    [StringLength(128)] public string? Contrasena { get; set; }
    
}


