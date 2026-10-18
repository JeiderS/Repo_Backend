using Inventory.Domain.Common.Results.Errors;

namespace Inventory.Application.Actions.Errors;

public static class ActionErrorBuilder
{
    public static Error ModuleNotFound() =>
        Error.CreateInstance(
            "ACTION_MODULE_NOT_FOUND",
            "El módulo indicado no existe.",
            ErrorType.NotFound);

    public static Error CodeAlreadyExists() =>
        Error.CreateInstance(
            "ACTION_CODE_EXISTS",
            "Ya existe una acción con ese código para el módulo indicado.",
            ErrorType.Conflict);

    public static Error CreationException() =>
        Error.CreateInstance(
            "ACTION_CREATION_ERROR",
            "No se pudo registrar la acción.",
            ErrorType.Failure);

    public static Error ActionNotFound() =>
        Error.CreateInstance(
            "ACTION_NOT_FOUND",
            "La acción indicada no existe.",
            ErrorType.NotFound);

    public static Error InUseByModules() =>
        Error.CreateInstance(
            "ACTION_IN_USE",
            "No se puede eliminar: la acción está asignada a uno o más módulos. Quítala de esos módulos primero.",
            ErrorType.Conflict);
}
