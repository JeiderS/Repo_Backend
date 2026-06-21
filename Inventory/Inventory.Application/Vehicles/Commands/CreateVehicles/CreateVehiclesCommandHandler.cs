using AutoMapper;
using MediatR;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using Inventory.Domain.Vehicles.DomainVehicles;
using Inventory.Domain.Vehicles.Entity;

namespace Inventory.Application.Vehicles.Commands.CreateVehicles;

public class CreateVehiclesCommandHandler(
    IVehiclesCreateService vehiclesCreateService,
    IMapper mapper) : IRequestHandler<CreateVehiclesCommand, Result<VoidResult, Error>>
{
    public async Task<Result<VoidResult, Error>> Handle(CreateVehiclesCommand request, CancellationToken cancellationToken)
    {
        var vehiclesEntity = mapper.Map<VehiclesEntity>(request.Request);
        var result = await vehiclesCreateService.CreateAsync(vehiclesEntity);

        if (!result.IsSuccess)
            return result.Error!;

        return result.Value!;
    }
}
