using FluentValidation;
using Inventory.Application.Common.Interfaces;
using Inventory.Application.Common.Media;

namespace Inventory.Application.Products.Command.UpdateProduct;

public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator(ICloudinaryImageUploader imageUploader)
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Description).MaximumLength(500);
        RuleFor(x => x.Price).GreaterThan(0);
        RuleFor(x => x.CategoryId).GreaterThan(0);

        this.ApplyImageRules(x => x.Image, imageUploader.MaxSizeBytes);
    }
}
