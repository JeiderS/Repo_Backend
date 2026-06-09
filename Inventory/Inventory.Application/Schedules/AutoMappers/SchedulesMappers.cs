using AutoMapper;
using FleetManager.Application.Schedules.Commands.CreateSchedules;
using FleetManager.Application.Schedules.Commands.UpdateSchedules;
using FleetManager.Application.Schedules.Dto;
using FleetManager.Domain.Schedules.DomainSchedules;
using FleetManager.Domain.Schedules.Entity;

namespace FleetManager.Application.Schedules.AutoMappers
{
    public class SchedulesMappers : Profile
    {
        public SchedulesMappers()
        {
            CreateMap<SchedulesEntity, SchedulesDto>().ReverseMap();
            CreateMap<SchedulesEntity, CreateSchedulesCommand>().ReverseMap();
            CreateMap<SchedulesEntity, UpdateSchedulesCommand>().ReverseMap();
        }
    }
}
