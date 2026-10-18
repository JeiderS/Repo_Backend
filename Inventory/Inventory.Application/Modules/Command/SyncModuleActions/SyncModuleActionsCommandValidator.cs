using FluentValidation;

namespace Inventory.Application.Modules.Command.SyncModuleActions;

public class SyncModuleActionsCommandValidator : AbstractValidator<SyncModuleActionsCommand>
{
    public SyncModuleActionsCommandValidator()
    {
        RuleFor(x => x.ModuleId).GreaterThan(0);

        // Lista vacía es válida: significa "quitar todas las acciones del módulo".
        RuleFor(x => x.ActionIds).NotNull();
        RuleForEach(x => x.ActionIds).GreaterThan(0);
    }
}
