using Application.DTO;
using FluentValidation;

namespace Application.Validators;

public  class CreateCuentaValidator : AbstractValidator<CreateCuentaDto>
{
    public CreateCuentaValidator()
    {
        RuleFor(x => x.Numero).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Tipo).Must(v => v == 1 || v == 2)
            .WithMessage("TipoCuenta inválido (1=Ahorros, 2=Corriente)");
        RuleFor(x => x.SaldoInicial).GreaterThanOrEqualTo(0);
        RuleFor(x => x.ClienteId).GreaterThan(0);
    }
}