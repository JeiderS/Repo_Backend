using MediatR;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;

namespace Inventory.Application.Vehicles.Commands.UpdateVehicles;

public record UpdateVehiclesCommand(
    int Id,
    string? Description,
    int Year,
    string? Make,
    string? Capacity,
    bool Active
) : IRequest<Result<VoidResult, Error>>;
