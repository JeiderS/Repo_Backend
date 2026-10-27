namespace Inventory.Domain.Products.Entity
{
    public class ProductEntity
    {
        // Para EF Core.
        private ProductEntity() { }

        public int Id { get; private set; }
        public string? ImageUrl { get; private set; }
        public string Name { get; private set; } = default!;
        public string? Description { get; private set; }
        public decimal Price { get; private set; }
        public int CategoryId { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        /// <summary>Regla de negocio: todo producto nuevo empieza activo.</summary>
        public static ProductEntity Create(string name, string? description, decimal price, int categoryId, string? imageUrl) =>
            new()
            {
                Name = name,
                Description = description,
                Price = price,
                CategoryId = categoryId,
                ImageUrl = imageUrl,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
            };

        public void Update(string name, string? description, decimal price, int categoryId, string? imageUrl)
        {
            Name = name;
            Description = description;
            Price = price;
            CategoryId = categoryId;
            ImageUrl = imageUrl;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Activate()
        {
            IsActive = true;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Deactivate()
        {
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
