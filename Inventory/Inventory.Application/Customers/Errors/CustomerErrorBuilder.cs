using Inventory.Domain.Common.Results.Errors;

namespace Inventory.Application.Customers.Errors;

public static class CustomerErrorBuilder
{
    public static Error CustomerNotFound() =>
        Error.CreateInstance(
            "CUSTOMER_NOT_FOUND",
            "El cliente indicado no existe.",
            ErrorType.NotFound);
}
