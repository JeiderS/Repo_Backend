using FluentValidation;

namespace Inventory.Application.Modules.Command.UpdateModule;

public class UpdateModuleCommandValidator : AbstractValidator<UpdateModuleCommand>
{
    public UpdateModuleCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Icon).MaximumLength(100);
        RuleFor(x => x.Route).MaximumLength(200);
        RuleFor(x => x.ParentId).GreaterThan(0).When(x => x.ParentId is not null);
        RuleFor(x => x.SortOrder).GreaterThanOrEqualTo(0);
    }
}
