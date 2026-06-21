using Inventory.Domain.Common.Pagination;
using MediatR;
using Inventory.Application.Drivers.Dto;


namespace Inventory.Application.Drivers.Query;

public record GetAllDriversQuery(PaginationParams PaginationParams) : IRequest<IEnumerable<DriversDto>>;

