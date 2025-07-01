using System.Net;
using FleetManager.Domain.Common.Results.Errors;

namespace FleetManager.Application.Routes.Errors;

public class RoutesErrorBuilder : IError
{
    public const string ROUTE_CREATION_ERROR = "RouteCreationErrorException";
    public const string ROUTE_NOT_FOUND_ERROR = "RouteNotFoundErrorException";
    public const string ROUTE_UPDATE_ERROR = "RouteUpdateErrorException";
    public static readonly string ROUTE_DELETE_ERROR = "RouteDeleteErrorException";

    public static Error RouteCreationException() => Error.CreateInstance(
        ROUTE_CREATION_ERROR,
        "Failed to create Route due to an internal error.",
        HttpStatusCode.InternalServerError);

    public static Error RouteUpdateException() => Error.CreateInstance(
        ROUTE_UPDATE_ERROR,
        "Failed to update Route due to an internal error.",
        HttpStatusCode.InternalServerError);

    public static Error RouteNotFoundException(int id) => Error.CreateInstance(
        ROUTE_NOT_FOUND_ERROR,
        $"Route with ID {id} was not found.",
        HttpStatusCode.NotFound);

    public static Error RouteNotFound(int id) => Error.CreateInstance(
        ROUTE_NOT_FOUND_ERROR,
        $"Route with ID {id} was not found.",
        HttpStatusCode.NotFound);

    public static Error RouteDeleteException() => Error.CreateInstance(
        ROUTE_DELETE_ERROR,
        "Failed to delete Route due to an internal error.",
        HttpStatusCode.InternalServerError);
}
