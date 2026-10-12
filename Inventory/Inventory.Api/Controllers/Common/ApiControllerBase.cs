using Inventory.Api.Common.Responses;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Api.Controllers.Common;

/// <summary>
/// Base de todos los controllers. Concentra en un solo lugar cómo se
/// convierte un resultado de Application en una respuesta HTTP, para que
/// cada controller no repita el mismo Match(...) una y otra vez.
/// </summary>
[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    /// <summary>Respuesta exitosa con datos.</summary>
    protected IActionResult Success(object? data, int statusCode = StatusCodes.Status200OK) =>
        StatusCode(statusCode, ResponseApiService.Response(statusCode, data));

    /// <summary>
    /// Convierte un Result en respuesta: si salió bien, devuelve los datos con
    /// <paramref name="successStatusCode"/>; si no, traduce el ErrorType a su
    /// código HTTP.
    /// </summary>
    protected IActionResult FromResult<T>(Result<T, Error> result, int successStatusCode = StatusCodes.Status200OK) =>
        result.Match(
            value => Success(value, successStatusCode),
            error =>
            {
                var statusCode = error.Type.ToStatusCode();
                return StatusCode(statusCode, ResponseApiService.Response(statusCode, message: error.Description));
            });
}
