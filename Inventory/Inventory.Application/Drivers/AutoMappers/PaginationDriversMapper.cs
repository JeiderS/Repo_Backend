using AutoMapper;
using Inventory.Domain.Common.Pagination;
using Inventory.Domain.Drivers.Entity;
using Inventory.Application.Drivers.Dto;

namespace Inventory.Application.Drivers.AutoMappers
{
    public class PaginationDriversMapper : Profile
    {
        public PaginationDriversMapper()
        {
            CreateMap<PaginationResponse<DriversEntity>, PaginationResponseDto<DriversDto>>().ReverseMap();
        }
    }
}

