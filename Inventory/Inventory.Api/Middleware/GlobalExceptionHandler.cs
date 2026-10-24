using FluentValidation;
using Inventory.Api.Common.Responses;
using Inventory.Domain.Common.Persistence;
using Microsoft.AspNetCore.Diagnostics;

namespace Inventory.Api.Middleware;

/// <summary>
/// Atrapa cualquier excepción que se escape de un endpoint y responde con el
/// mismo sobre { statusCode, success, message, data } que el resto de la API,
/// en vez de un 500 genérico.
///
/// Las excepciones son para lo INESPERADO o lo que no puede expresarse con
/// Result (ver ValidationBehavior). Los errores esperados del negocio no
/// llegan aquí: viajan como Result y los traduce ApiControllerBase.
/// </summary>
public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        int statusCode;
        string message;
        object? data = null;

        switch (exception)
        {
            case ValidationException validationException:
                statusCode = StatusCodes.Status400BadRequest;
                message = "Los datos enviados no son válidos.";
                // { "Email": ["..."], "Password": ["...", "..."] }
                data = validationException.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
                break;

            case DuplicateRecordException:
                statusCode = StatusCodes.Status409Conflict;
                message = exception.Message;
                logger.LogWarning(exception, "Duplicate record rejected by the database");
                break;

            case ConcurrencyConflictException:
                statusCode = StatusCodes.Status409Conflict;
                message = exception.Message;
                logger.LogWarning(exception, "Concurrency conflict detected by the database");
                break;

            default:
                statusCode = StatusCodes.Status500InternalServerError;
                // Nunca se devuelve el detalle interno al cliente; queda en el log.
                message = "Ocurrió un error inesperado. Intente nuevamente.";
                logger.LogError(exception, "Unhandled exception");
                break;
        }

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(
            ResponseApiService.Response(statusCode, data, message),
            cancellationToken);

        return true;
    }
}
