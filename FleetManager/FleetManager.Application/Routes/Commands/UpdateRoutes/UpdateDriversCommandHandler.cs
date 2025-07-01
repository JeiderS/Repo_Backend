using AutoMapper;
using MediatR;
using FleetManager.Domain.Common.Results;
using FleetManager.Domain.Common.Results.Errors;
using FleetManager.Domain.Routes.DomainRoutes;
using FleetManager.Domain.Routes.Entity;

namespace FleetManager.Application.Routes.Commands.UpdateRoutes;

public class UpdateRoutesCommandHandler(IRoutesUpdateService routesUpdateService, IMapper mapper)
    : IRequestHandler<UpdateRoutesCommand, Result<VoidResult, Error>>
{
    public async Task<Result<VoidResult, Error>> Handle(UpdateRoutesCommand request, CancellationToken cancellationToken)
    {
        var routesEntity = mapper.Map<RoutesEntity>(request);
        var result = await routesUpdateService.UpdateAsync(routesEntity);

        if (!result.IsSuccess)
            return result.Error!;
        return result.Value!;
    }
}
