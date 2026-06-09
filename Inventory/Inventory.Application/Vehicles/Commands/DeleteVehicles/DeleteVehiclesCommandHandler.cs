using MediatR;
using FleetManager.Domain.Common.Results;
using FleetManager.Domain.Common.Results.Errors;
using FleetManager.Domain.Vehicles.DomainVehicles;
using FleetManager.Application.Vehicles.Commands.DeleteVehicles;

public class DeleteVehiclesCommandHandler(IVehiclesDeleteService vehiclesDeleteService)
    : IRequestHandler<DeleteVehiclesCommand, Result<VoidResult, Error>>
{
    public async Task<Result<VoidResult, Error>> Handle(DeleteVehiclesCommand request, CancellationToken cancellationToken)
    {
        var result = await vehiclesDeleteService.DeleteAsync(request.Id);

        if (!result.IsSuccess)
            return result.Error!;

        return result.Value!;
    }
}
