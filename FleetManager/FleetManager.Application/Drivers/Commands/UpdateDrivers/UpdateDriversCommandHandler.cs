using AutoMapper;
using MediatR;
using FleetManager.Domain.Common.Results;
using FleetManager.Domain.Common.Results.Errors;
using FleetManager.Domain.Drivers.DomainDrivers;
using FleetManager.Domain.Drivers.Entity;

namespace FleetManager.Application.Drivers.Commands.UpdateDrivers;

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