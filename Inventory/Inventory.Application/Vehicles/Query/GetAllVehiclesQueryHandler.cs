
using AutoMapper;
using MediatR;
using Inventory.Domain.Vehicles.DomainVehicles;
using Inventory.Application.Vehicles.Dto;


namespace Inventory.Application.Vehicles.Query
{
    public class GetAllVehiclesQueryHandler(
        IMapper mapper, 
        IVehiclesGetAllService vehiclesGetAllService) 
        : IRequestHandler<GetAllVehiclesQuery, IEnumerable<VehiclesDto>>
 
    {
        public async Task<IEnumerable<VehiclesDto>> Handle(GetAllVehiclesQuery request, CancellationToken cancellationToken)
        {
            var result = await vehiclesGetAllService.GetAllAsync(request.PaginationParams);
            return mapper.Map<List<VehiclesDto>>(result);
        }
    }
}