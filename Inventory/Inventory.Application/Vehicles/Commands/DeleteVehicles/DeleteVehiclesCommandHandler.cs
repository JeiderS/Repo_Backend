using MediatR;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using Inventory.Domain.Vehicles.DomainVehicles;
using Inventory.Application.Vehicles.Commands.DeleteVehicles;

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
