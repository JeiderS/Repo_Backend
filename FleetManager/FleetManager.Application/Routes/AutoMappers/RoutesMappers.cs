
using AutoMapper;
using Fleet.Application.Routes.Dto;
using Fleet.Domain.Routes.Entity;

namespace Fleet.Application.Routes.AutoMappers
{
    public class RoutesMappers : Profile
    {
        public RoutesMappers()
        {
            CreateMap<RoutesEntity, RoutesDto>().ReverseMap();

        }
    }
}


