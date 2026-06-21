using MediatR;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;

namespace Inventory.Application.Routes.Commands.UpdateRoutes;

public record UpdateRoutesCommand(
    int Id,
    string? Description,
    int DriverId,
    int VehicleId,
    string Origin,
    string Destination,
    TimeSpan StartTime,
    TimeSpan EndTime,
    bool Active
) : IRequest<Result<VoidResult, Error>>;
