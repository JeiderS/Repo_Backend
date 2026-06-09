
using AutoMapper;
using FleetManager.Application.Routes.Commands.CreateRoutes;
using FleetManager.Application.Routes.Commands.UpdateRoutes;
using FleetManager.Application.Routes.Dto;
using FleetManager.Domain.Routes.Entity;

namespace FleetManager.Application.Routes.AutoMappers
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


