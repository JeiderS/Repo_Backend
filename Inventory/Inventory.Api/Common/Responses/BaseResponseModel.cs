namespace Inventory.Api.Common.Responses;

/// <summary>
/// El "sobre" con el que la API envuelve todas sus respuestas:
/// { statusCode, success, message, data }.
/// Es un detalle de cómo responde la API por HTTP, por eso vive en la capa
/// Api y no en Domain ni en Application.
/// </summary>
public class BaseResponseModel
{
    public int StatusCode { get; set; }
    public bool Success { get; set; }
    public string? Message { get; set; }
    public object? Data { get; set; }
}
