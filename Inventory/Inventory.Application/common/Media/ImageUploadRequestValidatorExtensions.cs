using System.Linq.Expressions;
using FluentValidation;
using Inventory.Application.Common.Errors;

namespace Inventory.Application.Common.Media;

/// <summary>
/// Shared FluentValidation rules for an optional <see cref="ImageUploadRequest"/> —
/// applied the same way by every command that accepts one, instead of each
/// feature re-declaring its own size/format checks.
/// </summary>
public static class ImageUploadRequestValidatorExtensions
{
    public static void ApplyImageRules<T>(
        this AbstractValidator<T> validator,
        Expression<Func<T, ImageUploadRequest?>> selector,
        long maxSizeBytes)
    {
        var maxMb = maxSizeBytes / (1024 * 1024);

        validator.RuleFor(selector)
            .Must(image => image is null || image.Length > 0)
            .WithMessage("El archivo esta vacio.");

        validator.RuleFor(selector)
            .Must(image => image is null || image.Length <= maxSizeBytes)
            .WithMessage($"La imagen supera el tamano maximo permitido ({maxMb} MB).");

        validator.RuleFor(selector)
            .Must(image => image is null || ImageErrorBuilder.AllowedContentTypes.Contains(image.ContentType))
            .WithMessage("Formato de imagen no permitido. Usa JPG, PNG o WEBP.");
    }
}
