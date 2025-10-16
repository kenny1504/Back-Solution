
namespace Domain.Entities;

public class Cliente : Persona
{
    public string ClienteId { get; set; } = null!;
    public string ContrasenaHash { get; set; } = null!;
    public bool Activo { get; set; }
    public ICollection<Cuenta> Cuentas { get; set; } = new List<Cuenta>();
}