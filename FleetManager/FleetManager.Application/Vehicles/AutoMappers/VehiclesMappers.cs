
using AutoMapper;
using FleetManager.Application.Vehicles.Dto;
using FleetManager.Domain.Vehicles.Entity;


namespace FleetManager.Application.Vehicles.AutoMappers
{
    public class VehiclesMappers : Profile
    {
        public VehiclesMappers()
        {
            CreateMap<VehiclesEntity, VehiclesDto>().ReverseMap();

        }
    }

}

