using AutoMapper;
using MediatR;
using FleetManager.Domain.Common.Results;
using FleetManager.Domain.Common.Results.Errors;
using FleetManager.Domain.Vehicles.DomainVehicles;
using FleetManager.Domain.Vehicles.Entity;
using FleetManager.Application.Vehicles.Commands.UpdateVehicles;

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
