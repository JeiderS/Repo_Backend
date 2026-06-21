
using AutoMapper;
using Inventory.Application.Routes.Commands.CreateRoutes;
using Inventory.Application.Routes.Commands.UpdateRoutes;
using Inventory.Application.Routes.Dto;
using Inventory.Domain.Routes.Entity;

namespace Inventory.Application.Routes.AutoMappers
{
    public class RoutesMappers : Profile
    {
        public RoutesMappers()
        {
            CreateMap<RoutesEntity, RoutesDto>().ReverseMap();
            CreateMap<RoutesEntity, CreateRoutesRequestDto>().ReverseMap();
            CreateMap<RoutesEntity, UpdateRoutesCommand>().ReverseMap();

        }
    }
}


