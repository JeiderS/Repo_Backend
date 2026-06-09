using FleetManager.Domain.Common.Pagination;
using MediatR;
using FleetManager.Application.Drivers.Dto;


namespace FleetManager.Application.Drivers.Query;

public record GetAllDriversQuery(PaginationParams PaginationParams) : IRequest<IEnumerable<DriversDto>>;

