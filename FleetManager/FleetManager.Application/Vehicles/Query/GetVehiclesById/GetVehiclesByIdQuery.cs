using MediatR;
using FleetManager.Application.Vehicles.Dto;
using FleetManager.Domain.Common.Results;
using FleetManager.Domain.Common.Results.Errors;

namespace FleetManager.Application.Vehicles.Query.GetVehiclesById;

public record GetVehiclesByIdQuery(int Id) : IRequest<Result<VehiclesDto, Error>>;
