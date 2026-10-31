namespace Inventory.Application.InventoryMovements.Dto;

public class InventoryMovementDto
{
    public long Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = default!;
    public string MovementType { get; set; } = default!;
    public decimal Quantity { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
}
