using AutoMapper;
using MediatR;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using Inventory.Domain.Routes.DomainRoutes;
using Inventory.Domain.Routes.Entity;

namespace Inventory.Application.Routes.Commands.CreateRoutes;

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
