using AutoMapper;
using MediatR;
using Inventory.Application.Schedules.Dto;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using Inventory.Domain.Schedules.DomainSchedules;

namespace Inventory.Application.Schedules.Query.GetSchedulesById;

public class GetSchedulesByIdQueryHandler(
    ISchedulesGetByIdService schedulesGetByIdService,
    IMapper mapper)
    : IRequestHandler<GetSchedulesByIdQuery, Result<SchedulesDto, Error>>
{
    public async Task<Result<SchedulesDto, Error>> Handle(GetSchedulesByIdQuery request, CancellationToken cancellationToken)
    {
        var result = await schedulesGetByIdService.GetByIdAsync(request.Id);
        if (!result.IsSuccess)
            return result.Error!;

        return mapper.Map<SchedulesDto>(result.Value);
    }
}
