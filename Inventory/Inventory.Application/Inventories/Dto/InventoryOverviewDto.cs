namespace Inventory.Application.Inventories.Dto;

public class InventoryOverviewDto
{
    public int InventoryId { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = default!;
    public string CategoryName { get; set; } = default!;
    public decimal Quantity { get; set; }
}
