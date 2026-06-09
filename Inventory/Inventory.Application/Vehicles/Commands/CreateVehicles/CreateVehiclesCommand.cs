using MediatR;
using FleetManager.Domain.Common.Results;
using FleetManager.Domain.Common.Results.Errors;

namespace FleetManager.Application.Vehicles.Commands.CreateVehicles;

public class CreateVehiclesCommand : IRequest<Result<VoidResult, Error>>
{
    public CreateVehiclesRequestDto Request { get; set; } = default!;
}
