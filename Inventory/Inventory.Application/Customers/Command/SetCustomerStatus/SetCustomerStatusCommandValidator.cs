using FluentValidation;

namespace Inventory.Application.Customers.Command.SetCustomerStatus;

public class SetCustomerStatusCommandValidator : AbstractValidator<SetCustomerStatusCommand>
{
    public SetCustomerStatusCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}
