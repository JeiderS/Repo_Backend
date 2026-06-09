using AutoMapper;
using FleetManager.Domain.Common.Pagination;
using FleetManager.Domain.ScheduleView.Entity;
using FleetManager.Application.ScheduleView.Dto;

namespace FleetManager.Application.ScheduleView.AutoMappers
{
    public class PaginationScheduleViewMapper : Profile
    {
        public PaginationScheduleViewMapper()
        {
            CreateMap<PaginationResponse<ScheduleViewEntity>, PaginationResponseDto<ScheduleViewDto>>().ReverseMap();
        }
    }
}
