
using AutoMapper;
using FleetManager.Application.Drivers.Commands.CreateDrivers;
using FleetManager.Application.Drivers.Commands.UpdateDrivers;
using FleetManager.Application.Drivers.Dto;
using FleetManager.Domain.Drivers.Entity;

namespace FleetManager.Application.Drivers.AutoMappers
{
    public class DriversMappers : Profile
    {
        public DriversMappers()
        {
            CreateMap<DriversEntity, DriversDto>().ReverseMap();
            CreateMap<DriversEntity, CreateDriversRequestDto>().ReverseMap();
            CreateMap<DriversEntity, UpdateDriversCommand>().ReverseMap();


        }
    }
}


