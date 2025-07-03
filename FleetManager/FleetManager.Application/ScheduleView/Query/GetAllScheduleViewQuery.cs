using MediatR;
using FleetManager.Application.Schedules.Dto;
using FleetManager.Domain.Common.Pagination;
using System.Collections.Generic;
using FleetManager.Application.ScheduleView.Dto;

namespace FleetManager.Application.ScheduleView.Query
{
    public record GetAllScheduleViewQuery(PaginationParams PaginationParams) : IRequest<IEnumerable<ScheduleViewDto>>;
}
