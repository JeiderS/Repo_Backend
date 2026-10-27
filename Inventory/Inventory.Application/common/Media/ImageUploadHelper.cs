using Inventory.Application.Common.Errors;
using Inventory.Application.Common.Interfaces;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;

namespace Inventory.Application.Common.Media;

/// <summary>
/// Shared "upload the new image or keep the existing URL" logic for any
/// Create/Update handler that accepts an optional <see cref="ImageUploadRequest"/>.
/// Reused across modules so each feature doesn't re-implement the same
/// upload-or-keep branching.
/// </summary>
public static class ImageUploadHelper
{
    public static async Task<Result<string?, Error>> ResolveImageUrlAsync(
        ICloudinaryImageUploader uploader,
        ImageUploadRequest? request,
        string? existingImageUrl,
        CancellationToken cancellationToken)
    {
        if (request is null)
            return existingImageUrl;

        try
        {
            return await uploader.UploadAsync(request.Content, request.FileName, request.ContentType, cancellationToken);
        }
        catch (Exception)
        {
            return ImageErrorBuilder.UploadFailed();
        }
    }
}
