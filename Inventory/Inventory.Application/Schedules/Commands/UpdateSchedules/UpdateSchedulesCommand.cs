using MediatR;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;

namespace Inventory.Application.Schedules.Commands.UpdateSchedules;

public record UpdateSchedulesCommand(
    int Id,
    int RouteId,
    int WeekNum,
    DateTime FromDate,
    DateTime ToDate,
    string DayOfWeek
) : IRequest<Result<VoidResult, Error>>;
