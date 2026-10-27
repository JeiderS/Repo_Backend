namespace Inventory.Api.Controllers.Products;

/// <summary>
/// Multipart form shape for Create/Update: product fields plus an optional
/// image in the same request. Lives in Api because <see cref="IFormFile"/>
/// is an ASP.NET Core type — the Application layer only ever sees the
/// already-extracted <c>ImageUploadRequest</c>.
/// </summary>
public class ProductForm
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int CategoryId { get; set; }
    public IFormFile? Image { get; set; }
}
