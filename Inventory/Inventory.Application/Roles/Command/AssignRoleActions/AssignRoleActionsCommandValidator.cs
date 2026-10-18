using FluentValidation;

namespace Inventory.Application.Roles.Command.AssignRoleActions;

public class AssignRoleActionsCommandValidator : AbstractValidator<AssignRoleActionsCommand>
{
    public AssignRoleActionsCommandValidator()
    {
        RuleFor(x => x.RoleId).GreaterThan(0);

        // Lista vacía es válida: significa "quitarle todas las acciones".
        RuleFor(x => x.Grants).NotNull();
        RuleForEach(x => x.Grants).ChildRules(grant =>
        {
            grant.RuleFor(g => g.ModuleId).GreaterThan(0);
            grant.RuleFor(g => g.ActionId).GreaterThan(0);
        });
    }
}
