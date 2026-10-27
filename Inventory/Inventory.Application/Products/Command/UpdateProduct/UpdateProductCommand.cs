using Inventory.Application.Common.Media;
using Inventory.Application.Products.Dto;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using MediatR;

namespace Inventory.Application.Products.Command.UpdateProduct;

public class UpdateProductCommand : IRequest<Result<ProductDto, Error>>
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int CategoryId { get; set; }

    /// <summary>Optional new image. Omitted (null) keeps the product's current ImageUrl unchanged.</summary>
    public ImageUploadRequest? Image { get; set; }
}
