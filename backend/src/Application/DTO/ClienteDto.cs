namespace Application.DTO;

public record ClienteDto(
    int Id,
    string ClienteId,
    string Nombre,
    string Genero,
    int Edad,
    string Identificacion,
    string Direccion,
    string Telefono,
    bool Activo
);