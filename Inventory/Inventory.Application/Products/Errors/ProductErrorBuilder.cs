using Inventory.Domain.Common.Results.Errors;

namespace Inventory.Application.Products.Errors;

public static class ProductErrorBuilder
{
    public static Error ProductNotFound() =>
        Error.CreateInstance(
            "PRODUCT_NOT_FOUND",
            "El producto indicado no existe.",
            ErrorType.NotFound);

    public static Error CreationException() =>
        Error.CreateInstance(
            "PRODUCT_CREATION_ERROR",
            "No se pudo registrar el producto.",
            ErrorType.Failure);

    public static Error CategoryNotFound() =>
        Error.CreateInstance(
            "PRODUCT_CATEGORY_NOT_FOUND",
            "La categoria especificada no existe.",
            ErrorType.NotFound);
}
