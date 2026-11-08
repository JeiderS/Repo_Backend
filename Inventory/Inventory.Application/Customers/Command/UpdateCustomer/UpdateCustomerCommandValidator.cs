using FluentValidation;

namespace Inventory.Application.Customers.Command.UpdateCustomer;

public class UpdateCustomerCommandValidator : AbstractValidator<UpdateCustomerCommand>
{
    public UpdateCustomerCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);

        RuleFor(x => x.Name)
            .NotEmpty()
            .Must(s => !string.IsNullOrWhiteSpace(s))
            .MaximumLength(150);

        RuleFor(x => x.Phone)
            .NotEmpty()
            .Must(s => !string.IsNullOrWhiteSpace(s))
            .MaximumLength(30);

        RuleFor(x => x.Email)
            .EmailAddress()
            .When(x => !string.IsNullOrEmpty(x.Email));

        RuleFor(x => x.Address)
            .MaximumLength(250);
    }
}
