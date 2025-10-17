using Application.DTO;
using FluentValidation;

namespace Application.Validators;

public sealed class UpdateCuentaValidator : AbstractValidator<UpdateCuentaDto>
{
    public UpdateCuentaValidator()
    {
        RuleFor(x => x.Numero).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Tipo).Must(v => v == 1 || v == 2);
        RuleFor(x => x.SaldoInicial).GreaterThanOrEqualTo(0);
        RuleFor(x => x.clienteIdFk).GreaterThan(0);
    }
}
