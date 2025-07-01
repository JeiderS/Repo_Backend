using FluentValidation;
using FleetManager.Application.Routes.Commands.CreateRoutes;

public class CreateRoutesCommandValidator : AbstractValidator<CreateRoutesRequestDto>
{
    public CreateRoutesCommandValidator()
    {
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("La descripción es requerida.")
            .MaximumLength(255).WithMessage("Máximo 255 caracteres.");

        RuleFor(x => x.DriverId)
            .GreaterThan(0).WithMessage("El ID del conductor debe ser mayor a 0.");

        RuleFor(x => x.VehicleId)
            .GreaterThan(0).WithMessage("El ID del vehículo debe ser mayor a 0.");

        RuleFor(x => x.Active)
            .NotNull().WithMessage("El estado activo es requerido.");
    }
}
