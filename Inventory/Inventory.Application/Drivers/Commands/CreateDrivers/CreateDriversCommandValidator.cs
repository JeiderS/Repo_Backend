using FluentValidation;
using Inventory.Application.Drivers.Commands.CreateDrivers;

public class CreateDriversCommandValidator : AbstractValidator<CreateDriversRequestDto>
{
    public CreateDriversCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("El nombre es requerido.")
            .MaximumLength(100).WithMessage("Máximo 100 caracteres.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("El apellido es requerido.")
            .MaximumLength(100).WithMessage("Máximo 100 caracteres.");

        RuleFor(x => x.SSN)
            .NotEmpty().WithMessage("El SSN es requerido.")
            .MaximumLength(50).WithMessage("Máximo 50 caracteres.");

        RuleFor(x => x.Dob)
            .NotEmpty().WithMessage("La fecha de nacimiento es requerida.");

        RuleFor(x => x.Address)
            .MaximumLength(255).WithMessage("Máximo 255 caracteres.");

        RuleFor(x => x.City)
            .MaximumLength(100).WithMessage("Máximo 100 caracteres.");

        RuleFor(x => x.Zip)
            .MaximumLength(20).WithMessage("Máximo 20 caracteres.");

        RuleFor(x => x.Phone)
            .MaximumLength(20).WithMessage("Máximo 20 caracteres.");

        RuleFor(x => x.Active)
            .NotNull().WithMessage("El estado activo es requerido.");
    }
}
