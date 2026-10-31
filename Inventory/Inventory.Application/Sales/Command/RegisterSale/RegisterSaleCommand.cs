using Inventory.Application.Sales.Dto;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using MediatR;

namespace Inventory.Application.Sales.Command.RegisterSale;

public class RegisterSaleCommand : IRequest<Result<SaleDetailDto, Error>>
{
    public List<SaleLineItem> Lines { get; set; } = new();
}

/// <summary>
/// Sin UnitPrice: el precio real sale siempre del ProductEntity en el
/// backend, nunca del frontend (ver RegisterSaleCommandHandler, pasada 1).
/// </summary>
public class SaleLineItem
{
    public int ProductId { get; set; }
    public decimal Quantity { get; set; }
}
