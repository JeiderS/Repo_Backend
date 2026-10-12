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
        RuleFor(x => x.ModuleId).GreaterThan(0);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
    }
}
