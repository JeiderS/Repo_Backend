using AutoMapper;
using FleetManager.Domain.Common.Pagination;
using FleetManager.Domain.Routes.Entity;
using FleetManager.Application.Routes.Dto;

namespace FleetManager.Application.Routes.AutoMappers
{
    public class PaginationRoutesMapper : Profile
    {
        public PaginationRoutesMapper()
        {
            CreateMap<PaginationResponse<RoutesEntity>, PaginationResponseDto<RoutesDto>>().ReverseMap();
        }
    }
}

