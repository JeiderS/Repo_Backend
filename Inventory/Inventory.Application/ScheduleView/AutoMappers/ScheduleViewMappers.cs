using AutoMapper;
using Inventory.Application.ScheduleView.Dto;
using Inventory.Domain.ScheduleView.Entity;

namespace Inventory.Application.ScheduleView.AutoMappers
{
    public class ScheduleViewMappers : Profile
    {
        public ScheduleViewMappers()
        {
            CreateMap<ScheduleViewEntity, ScheduleViewDto>().ReverseMap();
        }
    }
}
