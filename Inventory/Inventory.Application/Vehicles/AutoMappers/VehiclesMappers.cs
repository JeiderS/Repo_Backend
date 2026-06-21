
using AutoMapper;
using Inventory.Application.Vehicles.Commands.CreateVehicles;
using Inventory.Application.Vehicles.Commands.UpdateVehicles;
using Inventory.Application.Vehicles.Dto;
using Inventory.Domain.Vehicles.Entity;


namespace Inventory.Application.Vehicles.AutoMappers
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

