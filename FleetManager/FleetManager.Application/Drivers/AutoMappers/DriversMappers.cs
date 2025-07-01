
using AutoMapper;
using FleetManager.Application.Drivers.Dto;
using FleetManager.Domain.Drivers.Entity;

namespace FleetManager.Application.Drivers.AutoMappers
{
    public class DriversMappers : Profile
    {
        public DriversMappers()
        {
            CreateMap<DriversEntity, DriversDto>().ReverseMap();

        }
    }
}


