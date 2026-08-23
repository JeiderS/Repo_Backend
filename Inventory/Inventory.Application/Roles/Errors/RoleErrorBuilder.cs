using System.Net;
using Inventory.Domain.Common.Results.Errors;

namespace Inventory.Application.Roles.Errors;

public static class RoleErrorBuilder
{
    public static Error NameAlreadyExists() =>
        Error.CreateInstance(
            "ROLE_NAME_EXISTS",
            "Ya existe un rol con ese nombre.",
            HttpStatusCode.Conflict);

    public static Error RoleNotFound() =>
        Error.CreateInstance(
            "ROLE_NOT_FOUND",
            "El rol indicado no existe.",
            HttpStatusCode.NotFound);

    public static Error ActionNotFound() =>
        Error.CreateInstance(
            "ROLE_ACTION_NOT_FOUND",
            "Una o más acciones indicadas no existen en el catálogo.",
            HttpStatusCode.BadRequest);

    public static Error CreationException() =>
        Error.CreateInstance(
            "ROLE_CREATION_ERROR",
            "No se pudo registrar el rol.",
            HttpStatusCode.InternalServerError);

    public static Error UpdateException() =>
        Error.CreateInstance(
            "ROLE_UPDATE_ERROR",
            "No se pudo actualizar el rol.",
            HttpStatusCode.InternalServerError);
}
