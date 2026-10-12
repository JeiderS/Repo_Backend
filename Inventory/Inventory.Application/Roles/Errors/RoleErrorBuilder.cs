using Inventory.Domain.Common.Results.Errors;

namespace Inventory.Application.Roles.Errors;

public static class RoleErrorBuilder
{
    public static Error NameAlreadyExists() =>
        Error.CreateInstance(
            "ROLE_NAME_EXISTS",
            "Ya existe un rol con ese nombre.",
            ErrorType.Conflict);

    public static Error RoleNotFound() =>
        Error.CreateInstance(
            "ROLE_NOT_FOUND",
            "El rol indicado no existe.",
            ErrorType.NotFound);

    public static Error ActionNotFound() =>
        Error.CreateInstance(
            "ROLE_ACTION_NOT_FOUND",
            "Una o más acciones indicadas no existen en el catálogo.",
            ErrorType.Validation);

    public static Error CreationException() =>
        Error.CreateInstance(
            "ROLE_CREATION_ERROR",
            "No se pudo registrar el rol.",
            ErrorType.Failure);

    public static Error UpdateException() =>
        Error.CreateInstance(
            "ROLE_UPDATE_ERROR",
            "No se pudo actualizar el rol.",
            ErrorType.Failure);
}
