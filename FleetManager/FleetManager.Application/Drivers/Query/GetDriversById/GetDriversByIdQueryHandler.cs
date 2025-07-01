using AutoMapper;
using MediatR;
using FleetManager.Application.Drivers.Dto;
using FleetManager.Domain.Common.Results;
using FleetManager.Domain.Common.Results.Errors;
using FleetManager.Domain.Drivers.DomainDrivers;

namespace FleetManager.Application.Drivers.Query.GetDriversById;

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