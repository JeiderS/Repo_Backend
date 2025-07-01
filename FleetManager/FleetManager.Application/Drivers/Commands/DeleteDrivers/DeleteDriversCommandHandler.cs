using AutoMapper;
using MediatR;
using FleetManager.Domain.Common.Results;
using FleetManager.Domain.Common.Results.Errors;
using FleetManager.Domain.Drivers.DomainDrivers;

namespace FleetManager.Application.Drivers.Commands.DeleteDrivers;

public class DeleteDriversCommandHandler(IDriversDeleteService driversDeleteService)
    : IRequestHandler<DeleteDriversCommand, Result<VoidResult, Error>>
{
    public async Task<Result<VoidResult, Error>> Handle(DeleteDriversCommand request, CancellationToken cancellationToken)
    {
        var result = await driversDeleteService.DeleteAsync(request.Id);

        if (!result.IsSuccess)
            return result.Error!;
        return result.Value!;
    }
}
