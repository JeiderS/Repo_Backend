using Inventory.Application.Products.Dto;
using Inventory.Application.Products.Errors;
using Inventory.Domain.Common.Persistence;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using Inventory.Domain.Products.Repositories;
using MediatR;

namespace Inventory.Application.Products.Command.SetProductStatus;

public class SetProductStatusCommandHandler(
    IProductRepository productRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<SetProductStatusCommand, Result<ProductDto, Error>>
{
    public async Task<Result<ProductDto, Error>> Handle(SetProductStatusCommand request, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(request.Id);
        if (product is null)
            return ProductErrorBuilder.ProductNotFound();

        if (request.IsActive)
            product.Activate();
        else
            product.Deactivate();

        // 0 filas guardadas NO es un error: si el estado no cambio, EF no
        // actualiza nada. Un fallo real llega como excepcion.
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ProductDto.FromEntity(product);
    }
}
