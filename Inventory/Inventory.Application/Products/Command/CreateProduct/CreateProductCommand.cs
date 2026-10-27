using Inventory.Application.Common.Media;
using Inventory.Application.Products.Dto;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using MediatR;

namespace Inventory.Application.Products.Command.CreateProduct;

public class CreateProductCommand : IRequest<Result<ProductDto, Error>>
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int CategoryId { get; set; }

    /// <summary>Optional image, sent in this same multipart request. Backend uploads it to Cloudinary.</summary>
    public ImageUploadRequest? Image { get; set; }
}
