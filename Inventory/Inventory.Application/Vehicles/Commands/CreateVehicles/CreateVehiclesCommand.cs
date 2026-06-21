using MediatR;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;

namespace Inventory.Application.Vehicles.Commands.CreateVehicles;

public class CreateVehiclesCommand : IRequest<Result<VoidResult, Error>>
{
    public CreateVehiclesRequestDto Request { get; set; } = default!;
}
