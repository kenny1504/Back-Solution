using Application.DTO;
using FluentValidation;

public class CreateClienteValidator : AbstractValidator<CreateClienteDto>
{
    public CreateClienteValidator()
    {
        RuleFor(x => x.ClienteId).NotEmpty().MaximumLength(30);
        RuleFor(x => x.Contrasena!)
            .MinimumLength(8)
            .Matches("[A-Z]").WithMessage("Debe contener al menos 1 mayúscula")
            .Matches("[a-z]").WithMessage("Debe contener al menos 1 minúscula")
            .Matches("[0-9]").WithMessage("Debe contener al menos 1 dígito")
            .Matches("[^a-zA-Z0-9]").WithMessage("Debe contener 1 caracter especial");
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(120);
        RuleFor(x => x.Genero).NotEmpty();
        RuleFor(x => x.Edad).InclusiveBetween(0, 120);
        RuleFor(x => x.Identificacion).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Telefono).NotEmpty().MaximumLength(30);
        RuleFor(x => x.Direccion).NotEmpty().MaximumLength(100);
        
    }
}