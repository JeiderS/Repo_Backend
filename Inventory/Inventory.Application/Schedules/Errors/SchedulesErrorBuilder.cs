using System.Net;
using FleetManager.Domain.Common.Results.Errors;

namespace FleetManager.Application.Schedules.Errors;

public class SchedulesErrorBuilder : IError
{
    public const string SCHEDULE_CREATION_ERROR = "ScheduleCreationErrorException";
    public const string SCHEDULE_NOT_FOUND_ERROR = "ScheduleNotFoundErrorException";
    public const string SCHEDULE_UPDATE_ERROR = "ScheduleUpdateErrorException";
    public static readonly string SCHEDULE_DELETE_ERROR = "ScheduleDeleteErrorException";

    public static Error ScheduleCreationException() => Error.CreateInstance(
        SCHEDULE_CREATION_ERROR,
        "Failed to create Schedule due to an internal error.",
        HttpStatusCode.InternalServerError);

    public static Error ScheduleUpdateException() => Error.CreateInstance(
        SCHEDULE_UPDATE_ERROR,
        "Failed to update Schedule due to an internal error.",
        HttpStatusCode.InternalServerError);

    public static Error ScheduleNotFoundException(int id) => Error.CreateInstance(
        SCHEDULE_NOT_FOUND_ERROR,
        $"Schedule with ID {id} was not found.",
        HttpStatusCode.NotFound);

    public static Error ScheduleNotFound(int id) => Error.CreateInstance(
        SCHEDULE_NOT_FOUND_ERROR,
        $"Schedule with ID {id} was not found.",
        HttpStatusCode.NotFound);

    public static Error ScheduleDeleteException() => Error.CreateInstance(
        SCHEDULE_DELETE_ERROR,
        "Failed to delete Schedule due to an internal error.",
        HttpStatusCode.InternalServerError);
}
