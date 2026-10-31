using Inventory.Application.Sales.Dto;
using Inventory.Domain.Sales.Repositories;
using MediatR;

namespace Inventory.Application.Sales.Query.GetSales;

public class GetSalesQueryHandler(ISaleRepository saleRepository)
    : IRequestHandler<GetSalesQuery, IReadOnlyList<SaleListDto>>
{
    public async Task<IReadOnlyList<SaleListDto>> Handle(GetSalesQuery request, CancellationToken cancellationToken)
    {
        var sales = await saleRepository.GetAllAsync();

        return sales
            .OrderByDescending(s => s.SaleDate)
            .Select(s => new SaleListDto
            {
                SaleId = s.Id,
                Number = s.Number,
                SaleDate = s.SaleDate,
                Origin = s.Origin,
                Total = s.Total,
            })
            .ToList();
    }
}
