
using AutoMapper;
using Inventory.Application.Routes.Dto;
using MediatR;
using Inventory.Domain.Routes.DomainRoutes;

namespace Inventory.Application.Routes.Query
{
    public class GetAllRoutesQueryHandler(IMapper mapper, 
        IRoutesGetAllService routesGetAllService) 
        : IRequestHandler<GetAllRoutesQuery, IEnumerable<RoutesDto>>
    {
        public async Task<IEnumerable<RoutesDto>> Handle(GetAllRoutesQuery request, CancellationToken cancellationToken)
        {
            var result = await routesGetAllService.GetAllAsync(request.PaginationParams);
            return mapper.Map<List<RoutesDto>>(result);
        }
    }
}
   