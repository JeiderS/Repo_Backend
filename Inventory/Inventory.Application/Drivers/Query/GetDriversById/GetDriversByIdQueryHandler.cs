using AutoMapper;
using MediatR;
using Inventory.Application.Drivers.Dto;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using Inventory.Domain.Drivers.DomainDrivers;

namespace Inventory.Application.Drivers.Query.GetDriversById;

public class GetDriversByIdQueryHandler(
    IDriversGetByIdService DriversGetByIdService,
    IMapper mapper)
    : IRequestHandler<GetDriversByIdQuery, Result<DriversDto, Error>>
{
    public async Task<Result<DriversDto, Error>> Handle(GetDriversByIdQuery request, CancellationToken cancellationToken)
    {
        var result = await DriversGetByIdService.GetByIdAsync(request.Id);
        if (!result.IsSuccess)
            return result.Error!;

        return mapper.Map<DriversDto>(result.Value);
    }
}