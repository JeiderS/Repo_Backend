using Inventory.Domain.Common.Results.Errors;

namespace Inventory.Application.Sales.Errors;

public static class SaleErrorBuilder
{
    public static Error SaleNotFound() =>
        Error.CreateInstance(
            "SALE_NOT_FOUND",
            "La venta especificada no existe.",
            ErrorType.NotFound);

    public static Error ProductNotFound() =>
        Error.CreateInstance(
            "SALE_PRODUCT_NOT_FOUND",
            "Uno de los productos de la venta no existe.",
            ErrorType.NotFound);

    public static Error InsufficientStock(string productName) =>
        Error.CreateInstance(
            "SALE_INSUFFICIENT_STOCK",
            $"No hay suficientes existencias de '{productName}' para completar la venta.",
            ErrorType.Conflict);
}
