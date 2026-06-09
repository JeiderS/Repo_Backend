using AutoMapper;
using MediatR;
using FleetManager.Application.ScheduleView.Dto;
using FleetManager.Domain.ScheduleView.DomainScheduleView;

namespace FleetManager.Application.ScheduleView.Query
{
    public class GetAllScheduleViewQueryHandler(IMapper mapper, IScheduleViewGetAllService scheduleViewGetAllService) : IRequestHandler<GetAllScheduleViewQuery, IEnumerable<ScheduleViewDto>>
    {
        public async Task<IEnumerable<ScheduleViewDto>> Handle(GetAllScheduleViewQuery request, CancellationToken cancellationToken)
        {
            var result = await scheduleViewGetAllService.GetAllAsync(request.PaginationParams);
            return mapper.Map<List<ScheduleViewDto>>(result);
        }
    }
}
