namespace Inventory.Api.Common.Responses;

public static class ResponseApiService
{
    public static BaseResponseModel Response(
        int statusCode,
        object? data = null,
        string? message = null)
    {
        return new BaseResponseModel
        {
            StatusCode = statusCode,
            Success = statusCode is >= 200 and < 300,
            Message = message,
            Data = data
        };
    }
}
