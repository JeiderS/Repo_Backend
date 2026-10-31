using Inventory.Application.Sales.Dto;
using Inventory.Application.Sales.Errors;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using Inventory.Domain.Products.Repositories;
using Inventory.Domain.SaleDetails.Repositories;
using Inventory.Domain.Sales.Repositories;
using MediatR;

namespace Inventory.Application.Sales.Query.GetSaleDetail;

public class GetSaleDetailQueryHandler(
    ISaleRepository saleRepository,
    ISaleDetailRepository saleDetailRepository,
    IProductRepository productRepository)
    : IRequestHandler<GetSaleDetailQuery, Result<SaleDetailDto, Error>>
{
    public async Task<Result<SaleDetailDto, Error>> Handle(GetSaleDetailQuery request, CancellationToken cancellationToken)
    {
        var sale = await saleRepository.GetByIdAsync(request.SaleId);
        if (sale is null)
            return Result<SaleDetailDto, Error>.Failure(SaleErrorBuilder.SaleNotFound());

        var details = await saleDetailRepository.GetBySaleIdAsync(request.SaleId);

        var lines = new List<SaleDetailLineDto>();
        foreach (var detail in details)
        {
            var product = await productRepository.GetByIdAsync(detail.ProductId);

            lines.Add(new SaleDetailLineDto
            {
                ProductId = detail.ProductId,
                ProductName = product?.Name ?? string.Empty,
                Quantity = detail.Quantity,
                UnitPrice = detail.UnitPrice,
                Subtotal = detail.Subtotal,
            });
        }

        return Result<SaleDetailDto, Error>.Success(new SaleDetailDto
        {
            SaleId = sale.Id,
            Number = sale.Number,
            SaleDate = sale.SaleDate,
            Origin = sale.Origin,
            Total = sale.Total,
            Lines = lines,
        });
    }
}
