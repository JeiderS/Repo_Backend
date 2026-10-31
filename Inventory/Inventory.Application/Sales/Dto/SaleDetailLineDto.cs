namespace Inventory.Application.Sales.Dto;

public class SaleDetailLineDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = default!;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Subtotal { get; set; }
}
