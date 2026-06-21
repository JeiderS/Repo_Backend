using AutoMapper;
using MediatR;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using Inventory.Domain.Drivers.DomainDrivers;
using Inventory.Domain.Drivers.Entity;

namespace Inventory.Application.Drivers.Commands.UpdateDrivers;

public class UpdateDriversCommandHandler(IDriversUpdateService DriversUpdateService, IMapper mapper) : IRequestHandler<UpdateDriversCommand, Result<VoidResult, Error>>
{
    public async Task<Result<VoidResult, Error>> Handle(UpdateDriversCommand request,
        CancellationToken cancellationToken)
    {
        var DriversEntity = mapper.Map<DriversEntity>(request);
        var result = await DriversUpdateService.UpdateAsync(DriversEntity);

        if (!result.IsSuccess)
            return result.Error!;
        return result.Value!;
    }
}