using AutoMapper;
using Inventory.Application.Schedules.Commands.CreateSchedules;
using Inventory.Application.Schedules.Commands.UpdateSchedules;
using Inventory.Application.Schedules.Dto;
using Inventory.Domain.Schedules.DomainSchedules;
using Inventory.Domain.Schedules.Entity;

namespace Inventory.Application.Schedules.AutoMappers
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
