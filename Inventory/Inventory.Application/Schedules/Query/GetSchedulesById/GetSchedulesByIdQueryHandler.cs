using AutoMapper;
using MediatR;
using FleetManager.Application.Schedules.Dto;
using FleetManager.Domain.Common.Results;
using FleetManager.Domain.Common.Results.Errors;
using FleetManager.Domain.Schedules.DomainSchedules;

namespace FleetManager.Application.Schedules.Query.GetSchedulesById;

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
