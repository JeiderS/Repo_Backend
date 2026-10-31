using Inventory.Application.Sales.Dto;
using MediatR;

namespace Inventory.Application.Sales.Query.GetSales;

public class GetSalesQuery : IRequest<IReadOnlyList<SaleListDto>>
{
}
