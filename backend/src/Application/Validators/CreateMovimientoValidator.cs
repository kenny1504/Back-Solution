using Application.DTO;
using FluentValidation;

namespace Application.Validators;

public sealed class CreateMovimientoValidator : AbstractValidator<CreateMovimientoDto>
{
  public CreateMovimientoValidator()
  {
    RuleFor(x => x.CuentaId).GreaterThan(0);
    RuleFor(x => x.Tipo).Must(t => t == 1 || t == 2)
      .WithMessage("Tipo inválido (1=Crédito, 2=Débito)");
    RuleFor(x => x.Valor).GreaterThan(0);
  }
}
