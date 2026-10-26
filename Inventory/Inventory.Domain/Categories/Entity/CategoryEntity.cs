namespace Inventory.Domain.Categories.Entity
{
    public class CategoryEntity
    {
        // Para EF Core.
        private CategoryEntity() { }

        public int Id { get; private set; }
        public string Name { get; private set; } = default!;
        public string? Description { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        /// <summary>Regla de negocio: toda categoría nueva empieza activa.</summary>
        public static CategoryEntity Create(string name, string? description) =>
            new()
            {
                Name = name,
                Description = description,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
            };

        public void Update(string name, string? description)
        {
            Name = name;
            Description = description;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Regla de negocio: desactivar una categoría ya inactiva no hace nada.
        /// </summary>
        /// <returns>true si el estado cambió; false si ya estaba inactiva.</returns>
        public bool Deactivate()
        {
            if (!IsActive)
                return false;

            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
            return true;
        }
    }
}
