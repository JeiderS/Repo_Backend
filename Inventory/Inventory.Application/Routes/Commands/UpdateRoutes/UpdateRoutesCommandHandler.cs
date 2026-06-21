using AutoMapper;
using MediatR;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using Inventory.Domain.Routes.DomainRoutes;
using Inventory.Domain.Routes.Entity;

namespace Inventory.Application.Routes.Commands.UpdateRoutes;

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
