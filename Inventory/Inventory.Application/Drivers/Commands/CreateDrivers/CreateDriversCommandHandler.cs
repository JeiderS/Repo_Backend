using AutoMapper;
using MediatR;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using Inventory.Domain.Drivers.DomainDrivers;
using Inventory.Domain.Drivers.Entity;

namespace Inventory.Application.Drivers.Commands.CreateDrivers;

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