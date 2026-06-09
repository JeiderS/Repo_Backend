using AutoMapper;
using MediatR;
using FleetManager.Domain.Common.Results;
using FleetManager.Domain.Common.Results.Errors;
using FleetManager.Domain.Vehicles.DomainVehicles;
using FleetManager.Domain.Vehicles.Entity;

namespace FleetManager.Application.Vehicles.Commands.CreateVehicles;

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
