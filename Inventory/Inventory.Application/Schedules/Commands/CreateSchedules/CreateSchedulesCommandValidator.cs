using FluentValidation;
using Inventory.Application.Schedules.Commands.CreateSchedules;

public class CreateSchedulesCommandValidator : AbstractValidator<CreateSchedulesRequestDto>
{
    public CreateSchedulesCommandValidator()
    {
        RuleFor(x => x.RouteId)
            .GreaterThan(0).WithMessage("El ID de la ruta es requerido y debe ser mayor a 0.");

        RuleFor(x => x.WeekNum)
            .InclusiveBetween(1, 53).WithMessage("El número de semana debe estar entre 1 y 53.");

        RuleFor(x => x.FromDate)
            .NotEmpty().WithMessage("La fecha de inicio es requerida.");

        RuleFor(x => x.ToDate)
            .NotEmpty().WithMessage("La fecha de fin es requerida.")
            .GreaterThanOrEqualTo(x => x.FromDate).WithMessage("La fecha de fin debe ser igual o posterior a la fecha de inicio.");

        RuleFor(x => x.DayOfWeek)
            .NotEmpty().WithMessage("El día de la semana es requerido.")
            .Must(BeAValidDay).WithMessage("El día debe ser uno de: Lunes, Martes, Miércoles, Jueves, Viernes, Sábado, Domingo.");
    }

    private bool BeAValidDay(string day)
    {
        var validDays = new[] { "Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado", "Domingo" };
        return validDays.Contains(day);
    }
}
