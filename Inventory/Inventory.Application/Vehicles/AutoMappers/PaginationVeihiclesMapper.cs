using AutoMapper;
using FleetManager.Domain.Common.Pagination;
using FleetManager.Domain.Vehicles.Entity;
using FleetManager.Application.Vehicles.Dto;

namespace FleetManager.Application.Vehicles.AutoMappers
{
    public class PaginationVeihiclesMapper : Profile
    {
        public PaginationVeihiclesMapper()
        {
            CreateMap<PaginationResponse<VehiclesEntity>, PaginationResponseDto<VehiclesDto>>().ReverseMap();
        }
    }
}

