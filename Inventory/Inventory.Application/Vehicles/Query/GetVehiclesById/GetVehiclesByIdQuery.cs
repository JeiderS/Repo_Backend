using MediatR;
using Inventory.Application.Vehicles.Dto;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;

namespace Inventory.Application.Vehicles.Query.GetVehiclesById;

public record GetVehiclesByIdQuery(int Id) : IRequest<Result<VehiclesDto, Error>>;
