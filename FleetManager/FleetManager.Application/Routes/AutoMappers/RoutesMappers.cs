
using AutoMapper;
using FleetManager.Application.Routes.Dto;
using FleetManager.Domain.Routes.Entity;

namespace FleetManager.Application.Routes.AutoMappers
{
    public class RoutesMappers : Profile
    {
        public RoutesMappers()
        {
            CreateMap<RoutesEntity, RoutesDto>().ReverseMap();

        }
    }
}


