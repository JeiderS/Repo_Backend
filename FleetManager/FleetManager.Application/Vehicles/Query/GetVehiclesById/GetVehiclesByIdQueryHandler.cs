using AutoMapper;
using MediatR;
using FleetManager.Application.Vehicles.Dto;
using FleetManager.Domain.Common.Results;
using FleetManager.Domain.Common.Results.Errors;
using FleetManager.Domain.Vehicles.DomainVehicles;

namespace FleetManager.Application.Vehicles.Query.GetVehiclesById;

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
