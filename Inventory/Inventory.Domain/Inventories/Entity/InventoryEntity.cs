namespace Inventory.Domain.Inventories.Entity
{
    public class InventoryEntity
    {
        // Para EF Core.
        private InventoryEntity() { }

        public int Id { get; private set; }
        public int ProductId { get; private set; }
        public decimal Quantity { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        public static InventoryEntity Create(int productId, decimal quantity) =>
            new()
            {
                ProductId = productId,
                Quantity = quantity,
                UpdatedAt = DateTime.UtcNow,
            };

        public void UpdateQuantity(decimal quantity)
        {
            Quantity = quantity;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
