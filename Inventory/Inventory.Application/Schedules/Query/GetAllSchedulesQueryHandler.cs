using AutoMapper;
using Inventory.Application.Schedules.Dto;
using MediatR;
using Inventory.Domain.Schedules.DomainSchedules;

namespace Inventory.Application.Schedules.Query
{
    public class GetAllSchedulesQueryHandler(
        IMapper mapper,
        ISchedulesGetAllService schedulesGetAllService)
        : IRequestHandler<GetAllSchedulesQuery, IEnumerable<SchedulesDto>>
    {
        public async Task<IEnumerable<SchedulesDto>> Handle(GetAllSchedulesQuery request, CancellationToken cancellationToken)
        {
            var result = await schedulesGetAllService.GetAllAsync(request.PaginationParams);
            return mapper.Map<List<SchedulesDto>>(result);
        }
    }
}
