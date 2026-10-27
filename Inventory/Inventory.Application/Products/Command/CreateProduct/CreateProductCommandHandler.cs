using Inventory.Application.Common.Interfaces;
using Inventory.Application.Common.Media;
using Inventory.Application.Products.Dto;
using Inventory.Application.Products.Errors;
using Inventory.Domain.Categories.Repositories;
using Inventory.Domain.Common.Persistence;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using Inventory.Domain.Products.Entity;
using Inventory.Domain.Products.Repositories;
using MediatR;

namespace Inventory.Application.Products.Command.CreateProduct;

public class CreateProductCommandHandler(
    IProductRepository productRepository,
    ICategoryRepository categoryRepository,
    ICloudinaryImageUploader imageUploader,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateProductCommand, Result<ProductDto, Error>>
{
    public async Task<Result<ProductDto, Error>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        if (!await categoryRepository.ExistsAsync(request.CategoryId))
            return ProductErrorBuilder.CategoryNotFound();

        var imageResult = await ImageUploadHelper.ResolveImageUrlAsync(
            imageUploader, request.Image, existingImageUrl: null, cancellationToken);
        if (!imageResult.IsSuccess)
            return imageResult.Error!;

        // ProductEntity.Create garantiza que el producto nace activo.
        var product = ProductEntity.Create(request.Name, request.Description, request.Price, request.CategoryId, imageResult.Value);

        await productRepository.AddAsync(product);

        var savedRows = await unitOfWork.SaveChangesAsync(cancellationToken);
        if (savedRows <= 0)
            return ProductErrorBuilder.CreationException();

        return ProductDto.FromEntity(product);
    }
}
