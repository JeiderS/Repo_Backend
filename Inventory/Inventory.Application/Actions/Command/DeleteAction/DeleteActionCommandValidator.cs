using FluentValidation;

namespace Inventory.Application.Actions.Command.DeleteAction;

public class DeleteActionCommandValidator : AbstractValidator<DeleteActionCommand>
{
    public DeleteActionCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("El id de la acción es obligatorio.");
    }
}
