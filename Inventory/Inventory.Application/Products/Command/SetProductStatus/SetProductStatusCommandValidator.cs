using FluentValidation;

namespace Inventory.Application.Products.Command.SetProductStatus;

public class SetProductStatusCommandValidator : AbstractValidator<SetProductStatusCommand>
{
    public SetProductStatusCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}
