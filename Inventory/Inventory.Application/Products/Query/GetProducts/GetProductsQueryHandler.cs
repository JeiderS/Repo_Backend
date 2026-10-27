using Inventory.Application.Products.Dto;
using Inventory.Domain.Products.Repositories;
using MediatR;

namespace Inventory.Application.Products.Query.GetProducts;

public class GetProductsQueryHandler(IProductRepository productRepository)
    : IRequestHandler<GetProductsQuery, IReadOnlyList<ProductDto>>
{
    public async Task<IReadOnlyList<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await productRepository.GetAllAsync();

        return products.Select(ProductDto.FromEntity).ToList();
    }
}
