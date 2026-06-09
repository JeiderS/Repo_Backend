
using FleetManager.Domain.Common.Pagination;
using MediatR;
using FleetManager.Application.Vehicles.Dto;

namespace FleetManager.Application.Vehicles.Query;

public record GetAllVehiclesQuery(PaginationParams PaginationParams) : IRequest<IEnumerable<VehiclesDto>>;

