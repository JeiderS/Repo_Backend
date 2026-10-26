using Inventory.Domain.Common.Results.Errors;

namespace Inventory.Application.Categories.Errors;

public static class CategoryErrorBuilder
{
    public static Error NameAlreadyExists() =>
        Error.CreateInstance(
            "CATEGORY_NAME_EXISTS",
            "Ya existe una categoria con ese nombre.",
            ErrorType.Conflict);

    public static Error CategoryNotFound() =>
        Error.CreateInstance(
            "CATEGORY_NOT_FOUND",
            "La categoria indicada no existe.",
            ErrorType.NotFound);

    public static Error CreationException() =>
        Error.CreateInstance(
            "CATEGORY_CREATION_ERROR",
            "No se pudo registrar la categoria.",
            ErrorType.Failure);
}
