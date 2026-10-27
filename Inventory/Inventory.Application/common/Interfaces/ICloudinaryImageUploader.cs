namespace Inventory.Application.Common.Interfaces;

public interface ICloudinaryImageUploader
{
    /// <summary>Configured max upload size in bytes, enforced by the Application-layer validator.</summary>
    long MaxSizeBytes { get; }

    Task<string> UploadAsync(Stream content, string fileName, string contentType, CancellationToken cancellationToken);
}
