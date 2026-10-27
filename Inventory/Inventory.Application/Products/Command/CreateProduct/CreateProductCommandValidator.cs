using FluentValidation;
using Inventory.Application.Common.Interfaces;
using Inventory.Application.Common.Media;

namespace Inventory.Application.Products.Command.CreateProduct;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator(ICloudinaryImageUploader imageUploader)
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Description).MaximumLength(500);
        RuleFor(x => x.Price).GreaterThan(0);
        RuleFor(x => x.CategoryId).GreaterThan(0);

        this.ApplyImageRules(x => x.Image, imageUploader.MaxSizeBytes);
    }
}
