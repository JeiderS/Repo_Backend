using Inventory.Domain.Products.Entity;

namespace Inventory.Application.Products.Dto;

public class ProductDto
{
    public int Id { get; set; }
    public string? ImageUrl { get; set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int CategoryId { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public static ProductDto FromEntity(ProductEntity product) => new()
    {
        Id = product.Id,
        ImageUrl = product.ImageUrl,
        Name = product.Name,
        Description = product.Description,
        Price = product.Price,
        CategoryId = product.CategoryId,
        IsActive = product.IsActive,
        CreatedAt = product.CreatedAt,
        UpdatedAt = product.UpdatedAt,
    };
}
