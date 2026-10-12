using FluentValidation;

namespace Inventory.Application.Users.Command.CreateUser;

/// <summary>
/// Validación de FORMA: que los datos lleguen completos y con el tamaño que
/// admite la base de datos. Las reglas que necesitan la BD (email repetido,
/// rol existente) las comprueba el handler.
/// </summary>
public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(100);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8).MaximumLength(100);
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Phone).MaximumLength(30);
        RuleFor(x => x.Address).MaximumLength(255);
        RuleFor(x => x.RoleId).GreaterThan(0).When(x => x.RoleId is not null);
    }
}
