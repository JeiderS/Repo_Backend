
using AutoMapper;
using Inventory.Application.Drivers.Commands.CreateDrivers;
using Inventory.Application.Drivers.Commands.UpdateDrivers;
using Inventory.Application.Drivers.Dto;
using Inventory.Domain.Drivers.Entity;

namespace Inventory.Application.Drivers.AutoMappers
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


