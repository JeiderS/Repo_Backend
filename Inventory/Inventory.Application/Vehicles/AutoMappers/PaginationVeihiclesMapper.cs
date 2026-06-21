using AutoMapper;
using Inventory.Domain.Common.Pagination;
using Inventory.Domain.Vehicles.Entity;
using Inventory.Application.Vehicles.Dto;

namespace Inventory.Application.Vehicles.AutoMappers
{
    public class PaginationVeihiclesMapper : Profile
    {
        public PaginationVeihiclesMapper()
        {
            CreateMap<PaginationResponse<VehiclesEntity>, PaginationResponseDto<VehiclesDto>>().ReverseMap();
        }
    }
}

