using FluentValidation;

namespace Inventory.Application.Inventories.Command.RegisterInventoryIn;

public class RegisterInventoryInCommandValidator : AbstractValidator<RegisterInventoryInCommand>
{
    public RegisterInventoryInCommandValidator()
    {
        RuleFor(x => x.InventoryId).GreaterThan(0);
        RuleFor(x => x.Quantity).GreaterThan(0);
        RuleFor(x => x.Description).MaximumLength(250);
    }
}
