using AutoMapper;
using MediatR;
using Inventory.Application.Vehicles.Dto;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using Inventory.Domain.Vehicles.DomainVehicles;

namespace Inventory.Application.Vehicles.Query.GetVehiclesById;

public class GetVehiclesByIdQueryHandler(
    IVehiclesGetByIdService vehiclesGetByIdService,
    IMapper mapper)
    : IRequestHandler<GetVehiclesByIdQuery, Result<VehiclesDto, Error>>
{
    public async Task<Result<VehiclesDto, Error>> Handle(GetVehiclesByIdQuery request, CancellationToken cancellationToken)
    {
        var result = await vehiclesGetByIdService.GetByIdAsync(request.Id);
        if (!result.IsSuccess)
            return result.Error!;

        return mapper.Map<VehiclesDto>(result.Value);
    }
}
