using Inventory.Application.Common.Interfaces;
using Inventory.Application.Common.Media;
using Inventory.Application.Products.Dto;
using Inventory.Application.Products.Errors;
using Inventory.Domain.Categories.Repositories;
using Inventory.Domain.Common.Persistence;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using Inventory.Domain.Products.Repositories;
using MediatR;

namespace Inventory.Application.Products.Command.UpdateProduct;

public class UpdateProductCommandHandler(
    IProductRepository productRepository,
    ICategoryRepository categoryRepository,
    ICloudinaryImageUploader imageUploader,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateProductCommand, Result<ProductDto, Error>>
{
    public async Task<Result<ProductDto, Error>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(request.Id);
        if (product is null)
            return ProductErrorBuilder.ProductNotFound();

        if (!await categoryRepository.ExistsAsync(request.CategoryId))
            return ProductErrorBuilder.CategoryNotFound();

        var imageResult = await ImageUploadHelper.ResolveImageUrlAsync(
            imageUploader, request.Image, product.ImageUrl, cancellationToken);
        if (!imageResult.IsSuccess)
            return imageResult.Error!;

        product.Update(request.Name, request.Description, request.Price, request.CategoryId, imageResult.Value);

        // 0 filas guardadas NO es un error: si los datos no cambiaron, EF no
        // actualiza nada. Un fallo real llega como excepcion.
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ProductDto.FromEntity(product);
    }
}
