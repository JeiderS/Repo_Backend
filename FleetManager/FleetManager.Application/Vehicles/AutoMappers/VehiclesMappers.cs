
using AutoMapper;
using FleetManager.Application.Vehicles.Commands.CreateVehicles;
using FleetManager.Application.Vehicles.Commands.UpdateVehicles;
using FleetManager.Application.Vehicles.Dto;
using FleetManager.Domain.Vehicles.Entity;


namespace FleetManager.Application.Vehicles.AutoMappers
{
    public class VehiclesMappers : Profile
    {
        public VehiclesMappers()
        {
            CreateMap<VehiclesEntity, VehiclesDto>().ReverseMap();
            CreateMap<VehiclesEntity, CreateVehiclesRequestDto>().ReverseMap();
            CreateMap<VehiclesEntity, UpdateVehiclesCommand>().ReverseMap();

        }
    }

}

