using Inventory.Application.Products.Dto;
using MediatR;

namespace Inventory.Application.Products.Query.GetProducts;

public record GetProductsQuery : IRequest<IReadOnlyList<ProductDto>>;
