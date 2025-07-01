using FluentValidation;
using FleetManager.Application.Vehicles.Commands.CreateVehicles;

public class CreateVehiclesCommandValidator : AbstractValidator<CreateVehiclesRequestDto>
{
    public CreateVehiclesCommandValidator()
    {
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("La descripción es requerida.")
            .MaximumLength(255).WithMessage("Máximo 255 caracteres.");

        RuleFor(x => x.Year)
            .GreaterThan(1900).WithMessage("El año debe ser válido (mayor a 1900).");

        RuleFor(x => x.Make)
            .NotEmpty().WithMessage("La marca es requerida.");

        RuleFor(x => x.Capacity)
            .NotEmpty().WithMessage("La capacidad es requerida.");

        RuleFor(x => x.Active)
            .NotNull().WithMessage("El estado activo es requerido.");
    }
}
