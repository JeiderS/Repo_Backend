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
}
