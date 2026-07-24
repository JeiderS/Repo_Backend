using System.Net;
using Inventory.Domain.Common.Results.Errors;

namespace Inventory.Application.Modules.Errors;

public static class ModuleErrorBuilder
{
    public static Error NameAlreadyExists() =>
        Error.CreateInstance(
            "MODULE_NAME_EXISTS",
            "Ya existe un módulo con ese nombre.",
            HttpStatusCode.Conflict);

    public static Error ModuleNotFound() =>
        Error.CreateInstance(
            "MODULE_NOT_FOUND",
            "El módulo indicado no existe.",
            HttpStatusCode.NotFound);

    public static Error ParentNotFound() =>
        Error.CreateInstance(
            "MODULE_PARENT_NOT_FOUND",
            "El módulo padre indicado no existe.",
            HttpStatusCode.NotFound);

    public static Error InvalidParent() =>
        Error.CreateInstance(
            "MODULE_INVALID_PARENT",
            "Un módulo no puede ser padre de sí mismo.",
            HttpStatusCode.BadRequest);

    public static Error CreationException() =>
        Error.CreateInstance(
            "MODULE_CREATION_ERROR",
            "No se pudo registrar el módulo.",
            HttpStatusCode.InternalServerError);

    public static Error UpdateException() =>
        Error.CreateInstance(
            "MODULE_UPDATE_ERROR",
            "No se pudo actualizar el módulo.",
            HttpStatusCode.InternalServerError);
}
