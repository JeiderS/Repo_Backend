using AutoMapper;
using MediatR;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using Inventory.Domain.Drivers.DomainDrivers;

namespace Inventory.Application.Drivers.Commands.DeleteDrivers;

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
