using AutoMapper;
using MediatR;
using FleetManager.Domain.Common.Results;
using FleetManager.Domain.Common.Results.Errors;
using FleetManager.Domain.Routes.DomainRoutes;
using FleetManager.Domain.Routes.Entity;

namespace FleetManager.Application.Routes.Commands.CreateRoutes;

public class CreateRoutesCommandHandler(
    IRoutesCreateService RoutesCreateService,
    IMapper mapper) : IRequestHandler<CreateRoutesCommand, Result<VoidResult, Error>>
{
    public async Task<Result<VoidResult, Error>> Handle(CreateRoutesCommand request, CancellationToken cancellationToken)
    {
        var RoutesEntity = mapper.Map<RoutesEntity>(request.Request);
        var result = await RoutesCreateService.CreateAsync(RoutesEntity);
        if (!result.IsSuccess)
            return result.Error!;

        return result.Value!;
    }
}
