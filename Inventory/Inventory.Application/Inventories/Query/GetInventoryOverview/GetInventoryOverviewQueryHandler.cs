using Inventory.Application.Inventories.Dto;
using Inventory.Domain.Categories.Repositories;
using Inventory.Domain.Inventories.Repositories;
using Inventory.Domain.Products.Repositories;
using MediatR;

namespace Inventory.Application.Inventories.Query.GetInventoryOverview;

public class GetInventoryOverviewQueryHandler(
    IInventoryRepository inventoryRepository,
    IProductRepository productRepository,
    ICategoryRepository categoryRepository)
    : IRequestHandler<GetInventoryOverviewQuery, IReadOnlyList<InventoryOverviewDto>>
{
    public async Task<IReadOnlyList<InventoryOverviewDto>> Handle(GetInventoryOverviewQuery request, CancellationToken cancellationToken)
    {
        var inventories = await inventoryRepository.GetAllAsync();
        var products = await productRepository.GetAllAsync();
        var categories = await categoryRepository.GetAllAsync();

        var productsById = products.ToDictionary(p => p.Id);
        var categoriesById = categories.ToDictionary(c => c.Id);

        return inventories
            .Select(inventory =>
            {
                productsById.TryGetValue(inventory.ProductId, out var product);
                var categoryName = product is not null && categoriesById.TryGetValue(product.CategoryId, out var category)
                    ? category.Name
                    : string.Empty;

                return new InventoryOverviewDto
                {
                    InventoryId = inventory.Id,
                    ProductId = inventory.ProductId,
                    ProductName = product?.Name ?? string.Empty,
                    CategoryName = categoryName,
                    Quantity = inventory.Quantity,
                };
            })
            .ToList();
    }
}
