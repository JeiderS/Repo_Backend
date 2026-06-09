using AutoMapper;
using FleetManager.Domain.Common.Pagination;
using FleetManager.Domain.Schedules.Entity;
using FleetManager.Application.Schedules.Dto;

namespace FleetManager.Application.Schedules.AutoMappers
{
    public class PaginationSchedulesMapper : Profile
    {
        public PaginationSchedulesMapper()
        {
            CreateMap<PaginationResponse<SchedulesEntity>, PaginationResponseDto<SchedulesDto>>().ReverseMap();
        }
    }
}
