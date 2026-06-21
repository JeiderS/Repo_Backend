using AutoMapper;
using MediatR;
using Inventory.Application.Routes.Dto;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using Inventory.Domain.Routes.DomainRoutes;

namespace Inventory.Application.Routes.Query.GetRoutesById;

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
