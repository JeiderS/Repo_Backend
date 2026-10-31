using Inventory.Application.Common.Interfaces;
using Inventory.Application.Common.Media;
using Inventory.Application.Products.Dto;
using Inventory.Application.Products.Errors;
using Inventory.Domain.Categories.Repositories;
using Inventory.Domain.Common.Persistence;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using Inventory.Domain.Inventories.Entity;
using Inventory.Domain.Inventories.Repositories;
using Inventory.Domain.Products.Entity;
using Inventory.Domain.Products.Repositories;
using MediatR;

namespace Inventory.Application.Products.Command.CreateProduct;

public class CreateProductCommandHandler(
    IProductRepository productRepository,
    ICategoryRepository categoryRepository,
    IInventoryRepository inventoryRepository,
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

        return await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            // ProductEntity.Create garantiza que el producto nace activo.
            var product = ProductEntity.Create(request.Name, request.Description, request.Price, request.CategoryId, imageResult.Value);

            await productRepository.AddAsync(product);

            var savedRows = await unitOfWork.SaveChangesAsync(cancellationToken);
            if (savedRows <= 0)
                return Result<ProductDto, Error>.Failure(ProductErrorBuilder.CreationException());

            // Todo producto nace con su propio registro de inventario en 0:
            // así el módulo de Inventario nunca tiene que manejar "producto
            // sin inventario". Mismo commit que el producto (ver IUnitOfWork
            // .ExecuteInTransactionAsync): si algo falla, ninguno se guarda.
            await inventoryRepository.AddAsync(InventoryEntity.Create(product.Id, 0));
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<ProductDto, Error>.Success(ProductDto.FromEntity(product));
        });
    }
}
