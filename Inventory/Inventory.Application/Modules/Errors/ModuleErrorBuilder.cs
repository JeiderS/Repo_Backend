using Inventory.Domain.Common.Results.Errors;

namespace Inventory.Application.Modules.Errors;

public static class ModuleErrorBuilder
{
    public static Error NameAlreadyExists() =>
        Error.CreateInstance(
            "MODULE_NAME_EXISTS",
            "Ya existe un módulo con ese nombre.",
            ErrorType.Conflict);

    public static Error ModuleNotFound() =>
        Error.CreateInstance(
            "MODULE_NOT_FOUND",
            "El módulo indicado no existe.",
            ErrorType.NotFound);

    public static Error ParentNotFound() =>
        Error.CreateInstance(
            "MODULE_PARENT_NOT_FOUND",
            "El módulo padre indicado no existe.",
            ErrorType.NotFound);

    public static Error CreationException() =>
        Error.CreateInstance(
            "MODULE_CREATION_ERROR",
            "No se pudo registrar el módulo.",
            ErrorType.Failure);

    public static Error ActionNotFound() =>
        Error.CreateInstance(
            "MODULE_ACTION_NOT_FOUND",
            "Una o más acciones indicadas no existen en el catálogo.",
            ErrorType.Validation);

    public static Error ActionsInUseByRoles() =>
        Error.CreateInstance(
            "MODULE_ACTION_IN_USE",
            "No se pueden quitar una o más acciones porque están asignadas a uno o más roles.",
            ErrorType.Conflict);
}
