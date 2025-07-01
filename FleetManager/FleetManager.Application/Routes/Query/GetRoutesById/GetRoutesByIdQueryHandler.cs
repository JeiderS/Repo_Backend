using AutoMapper;
using MediatR;
using FleetManager.Application.Routes.Dto;
using FleetManager.Domain.Common.Results;
using FleetManager.Domain.Common.Results.Errors;
using FleetManager.Domain.Routes.DomainRoutes;

namespace FleetManager.Application.Routes.Query.GetRoutesById;

public class GetRoutesByIdQueryHandler(
    IRoutesGetByIdService routesGetByIdService,
    IMapper mapper)
    : IRequestHandler<GetRoutesByIdQuery, Result<RoutesDto, Error>>
{
    public async Task<Result<RoutesDto, Error>> Handle(GetRoutesByIdQuery request, CancellationToken cancellationToken)
    {
        var result = await routesGetByIdService.GetByIdAsync(request.Id);
        if (!result.IsSuccess)
            return result.Error!;

        return mapper.Map<RoutesDto>(result.Value);
    }
}
