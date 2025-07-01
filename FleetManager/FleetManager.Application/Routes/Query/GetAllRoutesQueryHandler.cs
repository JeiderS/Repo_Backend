
using AutoMapper;
using FleetManager.Application.Routes.Dto;
using MediatR;
using FleetManager.Domain.Routes.DomainRoutes;

namespace FleetManager.Application.Routes.Query
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
   