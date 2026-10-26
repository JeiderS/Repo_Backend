using FluentValidation;

namespace Inventory.Application.Categories.Command.SetCategoryStatus;

public class SetCategoryStatusCommandValidator : AbstractValidator<SetCategoryStatusCommand>
{
    public SetCategoryStatusCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}
