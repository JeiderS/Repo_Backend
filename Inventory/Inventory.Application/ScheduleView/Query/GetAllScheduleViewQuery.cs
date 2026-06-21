using MediatR;
using Inventory.Application.Schedules.Dto;
using Inventory.Domain.Common.Pagination;
using System.Collections.Generic;
using Inventory.Application.ScheduleView.Dto;

namespace Inventory.Application.ScheduleView.Query
{
    public record GetAllScheduleViewQuery(PaginationParams PaginationParams) : IRequest<IEnumerable<ScheduleViewDto>>;
}
