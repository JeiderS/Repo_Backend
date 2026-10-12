using FluentValidation;

namespace Inventory.Application.Modules.Command.CreateModule;

public class CreateModuleCommandValidator : AbstractValidator<CreateModuleCommand>
{
    public CreateModuleCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Icon).MaximumLength(100);
        RuleFor(x => x.Route).MaximumLength(200);
        RuleFor(x => x.ParentId).GreaterThan(0).When(x => x.ParentId is not null);
        RuleFor(x => x.SortOrder).GreaterThanOrEqualTo(0);
    }
}
