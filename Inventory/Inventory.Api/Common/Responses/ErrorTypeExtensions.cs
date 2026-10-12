using Inventory.Domain.Common.Results.Errors;

namespace Inventory.Api.Common.Responses;

/// <summary>
/// Traductor entre el idioma del negocio (ErrorType) y el idioma de HTTP
/// (códigos de estado). Es el ÚNICO lugar del sistema que sabe que
/// "NotFound" significa 404.
/// </summary>
public static class ErrorTypeExtensions
{
    public static int ToStatusCode(this ErrorType type) => type switch
    {
        ErrorType.Validation => StatusCodes.Status400BadRequest,
        ErrorType.NotFound => StatusCodes.Status404NotFound,
        ErrorType.Conflict => StatusCodes.Status409Conflict,
        ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
        _ => StatusCodes.Status500InternalServerError
    };
}
