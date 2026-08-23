using System.Net;
using Inventory.Domain.Common.Results.Errors;

namespace Inventory.Application.Users.Errors;

public static class UserErrorBuilder
{
    public static Error EmailAlreadyExists() =>
        Error.CreateInstance(
            "USER_EMAIL_EXISTS",
            "Ya existe un usuario con ese correo.",
            HttpStatusCode.Conflict);

    public static Error UserNotFound() =>
        Error.CreateInstance(
            "USER_NOT_FOUND",
            "El usuario indicado no existe.",
            HttpStatusCode.NotFound);

    public static Error RoleNotFound() =>
        Error.CreateInstance(
            "USER_ROLE_NOT_FOUND",
            "El rol indicado no existe.",
            HttpStatusCode.NotFound);

    public static Error CreationException() =>
        Error.CreateInstance(
            "USER_CREATION_ERROR",
            "No se pudo registrar el usuario.",
            HttpStatusCode.InternalServerError);

    public static Error UpdateException() =>
        Error.CreateInstance(
            "USER_UPDATE_ERROR",
            "No se pudo actualizar el usuario.",
            HttpStatusCode.InternalServerError);
}
