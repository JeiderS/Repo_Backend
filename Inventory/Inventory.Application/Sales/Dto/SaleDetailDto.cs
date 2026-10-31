namespace Inventory.Application.Sales.Dto;

public class SaleDetailDto
{
    public int SaleId { get; set; }
    public string Number { get; set; } = default!;
    public DateTime SaleDate { get; set; }
    public string Origin { get; set; } = default!;
    public decimal Total { get; set; }
    public List<SaleDetailLineDto> Lines { get; set; } = new();
}
