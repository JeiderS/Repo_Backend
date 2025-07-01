using AutoMapper;
using MediatR;
using FleetManager.Domain.Common.Results;
using FleetManager.Domain.Common.Results.Errors;
using FleetManager.Domain.Routes.DomainRoutes;

namespace FleetManager.Application.Routes.Commands.DeleteRoutes;

public class DeleteRoutesCommandHandler(IRoutesDeleteService routesDeleteService)
    : IRequestHandler<DeleteRoutesCommand, Result<VoidResult, Error>>
{
    public async Task<Result<VoidResult, Error>> Handle(DeleteRoutesCommand request, CancellationToken cancellationToken)
    {
        var result = await routesDeleteService.DeleteAsync(request.Id);

        if (!result.IsSuccess)
            return result.Error!;
        return result.Value!;
    }
}
