using Inventory.Domain.Common.Results.Errors;
using Inventory.Domain.Common.Results;
using MediatR;

namespace Inventory.Application.Vehicles.Commands.DeleteVehicles;

public record DeleteVehiclesCommand(int Id) : IRequest<Result<VoidResult, Error>>;
