using FluentValidation;

namespace Inventory.Application.Customers.Command.CreateCustomer;

public class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
{
    public CreateCustomerCommandValidator()
    {
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
