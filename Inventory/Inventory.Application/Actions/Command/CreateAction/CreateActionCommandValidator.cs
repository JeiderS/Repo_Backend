using FluentValidation;

namespace Inventory.Application.Actions.Command.CreateAction;

/// <summary>
/// Solo forma. Que el nombre produzca un código válido lo decide la entidad
/// (ActionEntity.Create), porque es una regla de negocio.
/// </summary>
public class CreateActionCommandValidator : AbstractValidator<CreateActionCommand>
{
    public CreateActionCommandValidator()
    {
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
