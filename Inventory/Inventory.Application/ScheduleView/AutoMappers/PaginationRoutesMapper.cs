using AutoMapper;
using Inventory.Domain.Common.Pagination;
using Inventory.Domain.ScheduleView.Entity;
using Inventory.Application.ScheduleView.Dto;

namespace Inventory.Application.ScheduleView.AutoMappers
{
    public class PaginationScheduleViewMapper : Profile
    {
        public PaginationScheduleViewMapper()
        {
            CreateMap<PaginationResponse<ScheduleViewEntity>, PaginationResponseDto<ScheduleViewDto>>().ReverseMap();
        }
    }
}
