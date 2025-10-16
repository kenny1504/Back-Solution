using System.ComponentModel.DataAnnotations;

namespace Application.DTO;

public class UpdateCuentaDto
{
    [Required, StringLength(20)]
    public string Numero { get; set; } = default!;

    [Required] public int Tipo { get; set; }
    
    [Range(0, double.MaxValue)]
    public decimal SaldoInicial { get; set; }

    public bool Activa { get; set; }
    [Required, Range(1, int.MaxValue)]
    public int ClienteId { get; set; }
}