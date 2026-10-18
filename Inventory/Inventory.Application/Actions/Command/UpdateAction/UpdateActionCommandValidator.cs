using FluentValidation;

namespace Inventory.Application.Actions.Command.UpdateAction;

/// <summary>
/// Solo forma. Mismas reglas que CreateActionCommandValidator, más la
/// validación del Id de ruta.
/// </summary>
public class UpdateActionCommandValidator : AbstractValidator<UpdateActionCommand>
{
    public UpdateActionCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("El id de la acción es obligatorio.");

        RuleFor(x => x.Code)
            .NotEmpty()
            .WithMessage("El código es obligatorio.")
            .Must(code => code == code.Trim())
            .WithMessage("El código no debe contener espacios al inicio o al final.")
            .MaximumLength(120)
            .WithMessage("El código no puede superar los 120 caracteres.")
            .Matches(@"^[A-Za-z][A-Za-z0-9]*$")
            .WithMessage("El código debe comenzar con una letra y contener solo letras y números.");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("El nombre es obligatorio.")
            .Must(name => name == name.Trim())
            .WithMessage("El nombre no debe contener espacios al inicio o al final.")
            .MaximumLength(100)
            .WithMessage("El nombre no puede superar los 100 caracteres.");
    }
}
