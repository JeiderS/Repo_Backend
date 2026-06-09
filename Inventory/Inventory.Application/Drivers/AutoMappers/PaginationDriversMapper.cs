using AutoMapper;
using FleetManager.Domain.Common.Pagination;
using FleetManager.Domain.Drivers.Entity;
using FleetManager.Application.Drivers.Dto;

namespace FleetManager.Application.Drivers.AutoMappers
{
    public class PaginationDriversMapper : Profile
    {
        public PaginationDriversMapper()
        {
            CreateMap<PaginationResponse<DriversEntity>, PaginationResponseDto<DriversDto>>().ReverseMap();
        }
    }
}

