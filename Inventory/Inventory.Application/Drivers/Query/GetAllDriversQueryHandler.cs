using AutoMapper;
using MediatR;
using FleetManager.Application.Drivers.Dto;
using FleetManager.Domain.Drivers.DomainDrivers;

namespace FleetManager.Application.Drivers.Query
{
    public class GetAllDriversQueryHandler(IMapper mapper, 
        IDriversGetAllService driversGetAllService) 
        : IRequestHandler<GetAllDriversQuery, IEnumerable<DriversDto>>
    {
        public async Task<IEnumerable<DriversDto>> Handle(GetAllDriversQuery request, CancellationToken cancellationToken)
        {
            var result = await driversGetAllService.GetAllAsync(request.PaginationParams);
            return mapper.Map<List<DriversDto>>(result);
        }
    }
}
