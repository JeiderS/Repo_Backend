using System.Net;
using Inventory.Domain.Common.Results.Errors;

namespace Inventory.Application.Actions.Errors;

public static class ActionErrorBuilder
{
    public static Error ModuleNotFound() =>
        Error.CreateInstance(
            "ACTION_MODULE_NOT_FOUND",
            "El módulo indicado no existe.",
            HttpStatusCode.NotFound);

    public static Error CodeAlreadyExists() =>
        Error.CreateInstance(
            "ACTION_CODE_EXISTS",
            "Ya existe una acción con ese código para el módulo indicado.",
            HttpStatusCode.Conflict);

    public static Error InvalidName() =>
        Error.CreateInstance(
            "ACTION_INVALID_NAME",
            "El nombre de la acción no es válido.",
            HttpStatusCode.BadRequest);

    public static Error CreationException() =>
        Error.CreateInstance(
            "ACTION_CREATION_ERROR",
            "No se pudo registrar la acción.",
            HttpStatusCode.InternalServerError);
}
