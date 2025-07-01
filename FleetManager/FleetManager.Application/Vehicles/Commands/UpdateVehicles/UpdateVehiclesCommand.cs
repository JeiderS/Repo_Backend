using MediatR;
using FleetManager.Domain.Common.Results;
using FleetManager.Domain.Common.Results.Errors;

namespace FleetManager.Application.Vehicles.Commands.UpdateVehicles;

public record UpdateVehiclesCommand(
    int Id,
    string? Description,
    int Year,
    string? Make,
    string? Capacity,
    bool Active
) : IRequest<Result<VoidResult, Error>>;
