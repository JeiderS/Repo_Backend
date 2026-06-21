using System.Net;
using Inventory.Domain.Common.Results.Errors;

namespace Inventory.Application.Vehicles.Errors;

public class VehiclesErrorBuilder : IError
{
    public const string VEHICLE_CREATION_ERROR = "VehicleCreationErrorException";
    public const string VEHICLE_NOT_FOUND_ERROR = "VehicleNotFoundErrorException";
    public const string VEHICLE_UPDATE_ERROR = "VehicleUpdateErrorException";
    public static readonly string VEHICLE_DELETE_ERROR = "VehicleDeleteErrorException";

    public static Error VehicleCreationException() => Error.CreateInstance(
        VEHICLE_CREATION_ERROR,
        "Failed to create Vehicle due to an internal error.",
        HttpStatusCode.InternalServerError);

    public static Error VehicleUpdateException() => Error.CreateInstance(
        VEHICLE_UPDATE_ERROR,
        "Failed to update Vehicle due to an internal error.",
        HttpStatusCode.InternalServerError);

    public static Error VehicleNotFoundException(int id) => Error.CreateInstance(
        VEHICLE_NOT_FOUND_ERROR,
        $"Vehicle with ID {id} was not found.",
        HttpStatusCode.NotFound);

    public static Error VehicleNotFound(int id) => Error.CreateInstance(
        VEHICLE_NOT_FOUND_ERROR,
        $"Vehicle with ID {id} was not found.",
        HttpStatusCode.NotFound);

    public static Error VehicleDeleteException() => Error.CreateInstance(
        VEHICLE_DELETE_ERROR,
        "Failed to delete Vehicle due to an internal error.",
        HttpStatusCode.InternalServerError);
}
