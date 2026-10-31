namespace Inventory.Application.Sales.Dto;

public class SaleListDto
{
    public int SaleId { get; set; }
    public string Number { get; set; } = default!;
    public DateTime SaleDate { get; set; }
    public string Origin { get; set; } = default!;
    public decimal Total { get; set; }
}
