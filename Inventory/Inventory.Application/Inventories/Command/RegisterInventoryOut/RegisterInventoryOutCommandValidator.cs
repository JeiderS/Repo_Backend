using FluentValidation;

namespace Inventory.Application.Inventories.Command.RegisterInventoryOut;

public class RegisterInventoryOutCommandValidator : AbstractValidator<RegisterInventoryOutCommand>
{
    public RegisterInventoryOutCommandValidator()
    {
        RuleFor(x => x.InventoryId).GreaterThan(0);
        RuleFor(x => x.Quantity).GreaterThan(0);
        RuleFor(x => x.Description).MaximumLength(250);
    }
}
