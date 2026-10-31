namespace Inventory.Domain.InventoryMovements.Entity
{
    public class InventoryMovementEntity
    {
        public const string MovementTypeIn = "IN";
        public const string MovementTypeOut = "OUT";

        // Para EF Core.
        private InventoryMovementEntity() { }

        public long Id { get; private set; }
        public int InventoryId { get; private set; }
        public string MovementType { get; private set; } = default!;
        public decimal Quantity { get; private set; }
        public string? Description { get; private set; }
        public DateTime CreatedAt { get; private set; }

        /// <summary>Regla de negocio: un movimiento es un registro operativo inmutable, sin Update ni Delete.</summary>
        public static InventoryMovementEntity Create(int inventoryId, string movementType, decimal quantity, string? description)
        {
            if (movementType != MovementTypeIn && movementType != MovementTypeOut)
                throw new ArgumentException("MovementType must be IN or OUT.", nameof(movementType));

            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));

            return new()
            {
                InventoryId = inventoryId,
                MovementType = movementType,
                Quantity = quantity,
                Description = description,
                CreatedAt = DateTime.UtcNow,
            };
        }
    }
}
