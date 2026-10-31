using Inventory.Application.Sales.Dto;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using MediatR;

namespace Inventory.Application.Sales.Query.GetSaleDetail;

public record GetSaleDetailQuery(int SaleId) : IRequest<Result<SaleDetailDto, Error>>;
