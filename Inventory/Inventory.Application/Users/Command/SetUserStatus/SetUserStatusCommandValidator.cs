using FluentValidation;

namespace Inventory.Application.Users.Command.SetUserStatus;

public class SetUserStatusCommandValidator : AbstractValidator<SetUserStatusCommand>
{
    public SetUserStatusCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}
