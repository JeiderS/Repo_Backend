
using AutoMapper;
using MediatR;
using FleetManager.Domain.Vehicles.DomainVehicles;
using FleetManager.Application.Vehicles.Dto;


namespace FleetManager.Application.Vehicles.Query
{
    public class GetAllVehiclesQueryHandler(IMapper mapper, IVehiclesGetAllService vehiclesGetAllService) : IRequestHandler<GetAllVehiclesQuery, IEnumerable<VehiclesDto>>
    {
        public async Task<IEnumerable<VehiclesDto>> Handle(GetAllVehiclesQuery request, CancellationToken cancellationToken)
        {
            var result = await vehiclesGetAllService.GetAllAsync(request.PaginationParams);
            return mapper.Map<List<VehiclesDto>>(result);
        }
    }
}