using AutoMapper;
using MediatR;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using Inventory.Domain.Vehicles.DomainVehicles;
using Inventory.Domain.Vehicles.Entity;
using Inventory.Application.Vehicles.Commands.UpdateVehicles;

public class UpdateVehiclesCommandHandler(IVehiclesUpdateService vehiclesUpdateService, IMapper mapper)
    : IRequestHandler<UpdateVehiclesCommand, Result<VoidResult, Error>>
{
    public async Task<Result<VoidResult, Error>> Handle(UpdateVehiclesCommand request, CancellationToken cancellationToken)
    {
        var vehiclesEntity = mapper.Map<VehiclesEntity>(request);
        var result = await vehiclesUpdateService.UpdateAsync(vehiclesEntity);

        if (!result.IsSuccess)
            return result.Error!;

        return result.Value!;
    }
}
