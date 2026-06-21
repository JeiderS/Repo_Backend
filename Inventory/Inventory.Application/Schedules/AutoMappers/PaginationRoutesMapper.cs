using AutoMapper;
using Inventory.Domain.Common.Pagination;
using Inventory.Domain.Schedules.Entity;
using Inventory.Application.Schedules.Dto;

namespace Inventory.Application.Schedules.AutoMappers
{
    public class PaginationSchedulesMapper : Profile
    {
        public PaginationSchedulesMapper()
        {
            CreateMap<PaginationResponse<SchedulesEntity>, PaginationResponseDto<SchedulesDto>>().ReverseMap();
        }
    }
}
