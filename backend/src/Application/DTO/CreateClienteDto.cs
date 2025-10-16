namespace Application.DTO;

public record CreateClienteDto(
    string ClienteId,
    string Contrasena,  // llega plano -> se hashea
    string Nombre,
    string Genero,
    int Edad,
    string Identificacion,
    string Direccion,
    string Telefono
);