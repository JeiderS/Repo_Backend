using AutoMapper;
using MediatR;
using FleetManager.Domain.Common.Results;
using FleetManager.Domain.Common.Results.Errors;
using FleetManager.Domain.Drivers.DomainDrivers;
using FleetManager.Domain.Drivers.Entity;

namespace FleetManager.Application.Drivers.Commands.CreateDrivers;

public class CreateDriversCommandHandler(
    IDriversCreateService DriversCreateService,
    IMapper mapper) : IRequestHandler<CreateDriversCommand, Result<VoidResult, Error>>
{
    public async Task<Result<VoidResult, Error>> Handle(CreateDriversCommand request, CancellationToken cancellationToken)
    {
        var DriversEntity = mapper.Map<DriversEntity>(request.Request);
        var result = await DriversCreateService.CreateAsync(DriversEntity);
        if (!result.IsSuccess)
            return result.Error!;

        return result.Value!;
    }
}