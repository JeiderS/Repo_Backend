using FleetManager.Domain.Common.Results.Errors;
using FleetManager.Domain.Common.Results;
using MediatR;

namespace FleetManager.Application.Vehicles.Commands.DeleteVehicles;

public record DeleteVehiclesCommand(int Id) : IRequest<Result<VoidResult, Error>>;
