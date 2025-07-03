using AutoMapper;
using FleetManager.Application.ScheduleView.Dto;
using FleetManager.Domain.ScheduleView.Entity;

namespace FleetManager.Application.ScheduleView.AutoMappers
{
    public class ScheduleViewMappers : Profile
    {
        public ScheduleViewMappers()
        {
            CreateMap<ScheduleViewEntity, ScheduleViewDto>().ReverseMap();
        }
    }
}
