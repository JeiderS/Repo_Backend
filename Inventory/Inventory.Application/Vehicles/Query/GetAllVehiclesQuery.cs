
using Inventory.Domain.Common.Pagination;
using MediatR;
using Inventory.Application.Vehicles.Dto;

namespace Inventory.Application.Vehicles.Query;

public record GetAllVehiclesQuery(PaginationParams PaginationParams) : IRequest<IEnumerable<VehiclesDto>>;

