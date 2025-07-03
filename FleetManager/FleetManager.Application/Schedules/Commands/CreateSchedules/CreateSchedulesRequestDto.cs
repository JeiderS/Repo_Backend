namespace FleetManager.Application.Schedules.Commands.CreateSchedules;

public record CreateSchedulesRequestDto(
    int RouteId,
    int WeekNum,
    DateTime FromDate,
    DateTime ToDate,
    string DayOfWeek
);
