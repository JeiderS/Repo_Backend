
using FleetManager.Domain.Common.Pagination;
using MediatR;
using FleetManager.Application.Vehicles.Dto;

namespace FleetManager.Application.Vehicles.Query;

public class GetAllVehiclesQuery(PaginationParams paginationParams) : IRequest<IEnumerable<VehiclesDto>>;

