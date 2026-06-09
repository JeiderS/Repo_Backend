using MediatR;
using FleetManager.Domain.Common.Results;
using FleetManager.Domain.Common.Results.Errors;

namespace FleetManager.Application.Schedules.Commands.UpdateSchedules;

public record UpdateSchedulesCommand(
    int Id,
    int RouteId,
    int WeekNum,
    DateTime FromDate,
    DateTime ToDate,
    string DayOfWeek
) : IRequest<Result<VoidResult, Error>>;
