using FluentValidation;

namespace Inventory.Application.Roles.Command.AssignRoleActions;

public class AssignRoleActionsCommandValidator : AbstractValidator<AssignRoleActionsCommand>
{
    public AssignRoleActionsCommandValidator()
    {
        RuleFor(x => x.RoleId).GreaterThan(0);

        // Una lista vacía es válida: significa "quitarle todas las acciones".
        RuleFor(x => x.ActionIds).NotNull();
        RuleForEach(x => x.ActionIds).GreaterThan(0);
    }
}
