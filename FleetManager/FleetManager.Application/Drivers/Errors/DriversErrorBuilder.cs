using System.Net;
using FleetManager.Domain.Common.Results.Errors;

namespace FleetManager.Application.Drivers.Errors;


public class DriversErrorBuilder : IError
{
    public const string DRIVER_CREATION_ERROR = "DriverCreationErrorException";
    public const string DRIVER_NOT_FOUND_ERROR = "DriverNotFoundErrorException";
    public const string DRIVER_UPDATE_ERROR = "DriverUpdateErrorException";
    public static readonly string DRIVER_DELETE_ERROR = "DriverDeleteErrorException";

    public static Error DriverCreationException() => Error.CreateInstance(
        DRIVER_CREATION_ERROR,
        "Failed to create Driver due to an internal error.",
        HttpStatusCode.InternalServerError);

    public static Error DriverUpdateException() => Error.CreateInstance(
        DRIVER_UPDATE_ERROR,
        "Failed to update Driver due to an internal error.",
        HttpStatusCode.InternalServerError);

    public static Error DriverNotFoundException(int id) => Error.CreateInstance(
        DRIVER_NOT_FOUND_ERROR,
        $"Driver with ID {id} was not found.",
        HttpStatusCode.NotFound);

    public static Error DriverNotFound(int id) => Error.CreateInstance(
    DRIVER_NOT_FOUND_ERROR,
    $"Driver with ID {id} was not found.",
    HttpStatusCode.NotFound);

    public static Error DriverDeleteException() => Error.CreateInstance(
        DRIVER_DELETE_ERROR,
        "Failed to delete Driver due to an internal error.",
        HttpStatusCode.InternalServerError);
}
