using System.Net;
using Inventory.Domain.Common.Results.Errors;

namespace Inventory.Application.Schedules.Errors
{
    public class ScheduleViewErrorBuilder : IError
    {
        public const string SCHEDULE_VIEW_RETRIEVE_ERROR = "ScheduleViewRetrieveErrorException";
        public const string SCHEDULE_VIEW_NOT_FOUND_ERROR = "ScheduleViewNotFoundErrorException";
        public const string SCHEDULE_VIEW_UPDATE_ERROR = "ScheduleViewUpdateErrorException";
        public static readonly string SCHEDULE_VIEW_DELETE_ERROR = "ScheduleViewDeleteErrorException";

        public static Error ScheduleViewRetrieveException() => Error.CreateInstance(
            SCHEDULE_VIEW_RETRIEVE_ERROR,
            "Failed to retrieve ScheduleView data due to an internal error.",
            HttpStatusCode.InternalServerError);

        public static Error ScheduleViewNotFoundException(string criteria) => Error.CreateInstance(
            SCHEDULE_VIEW_NOT_FOUND_ERROR,
            $"ScheduleView record matching criteria '{criteria}' was not found.",
            HttpStatusCode.NotFound);

        public static Error ScheduleViewUpdateException() => Error.CreateInstance(
            SCHEDULE_VIEW_UPDATE_ERROR,
            "Failed to update ScheduleView due to an internal error.",
            HttpStatusCode.InternalServerError);

        public static Error ScheduleViewDeleteException() => Error.CreateInstance(
            SCHEDULE_VIEW_DELETE_ERROR,
            "Failed to delete ScheduleView due to an internal error.",
            HttpStatusCode.InternalServerError);
    }
}
