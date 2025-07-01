using MediatR;
using FleetManager.Domain.Common.Results;
using FleetManager.Domain.Common.Results.Errors;

namespace FleetManager.Application.Routes.Commands.UpdateRoutes;

public record UpdateRoutesCommand(
    int Id,
    string? Description,
    int DriverId,
    int VehicleId,
    bool Active
) : IRequest<Result<VoidResult, Error>>;
