using AutoMapper;
using MediatR;
using Inventory.Application.Drivers.Dto;
using Inventory.Domain.Drivers.DomainDrivers;

namespace Inventory.Application.Drivers.Query
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
