using AutoMapper;
using MediatR;
using Inventory.Application.ScheduleView.Dto;
using Inventory.Domain.ScheduleView.DomainScheduleView;

namespace Inventory.Application.ScheduleView.Query
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
