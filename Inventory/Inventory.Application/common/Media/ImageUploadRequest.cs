namespace Inventory.Application.Common.Media;

/// <summary>
/// Carries an unprocessed image file through a command. Shared by any feature
/// that accepts an optional image alongside its other fields in the same
/// multipart request (Products today, future modules later) — not tied to
/// any single component or entity.
/// </summary>
public class ImageUploadRequest
{
    public required Stream Content { get; init; }
    public required string FileName { get; init; }
    public required string ContentType { get; init; }
    public required long Length { get; init; }
}
