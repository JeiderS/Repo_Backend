using Inventory.Domain.Common.Results.Errors;

namespace Inventory.Application.Common.Errors;

/// <summary>Generic image-upload errors/constraints, shared by every feature that accepts an image.</summary>
public static class ImageErrorBuilder
{
    public static readonly string[] AllowedContentTypes = ["image/jpeg", "image/png", "image/webp"];

    public static Error UploadFailed() =>
        Error.CreateInstance(
            "IMAGE_UPLOAD_FAILED",
            "No se pudo subir la imagen. Intenta nuevamente.",
            ErrorType.Failure);
}
