using AutoMapper;
using Inventory.Domain.Common.Pagination;
using Inventory.Domain.Routes.Entity;
using Inventory.Application.Routes.Dto;

namespace Inventory.Application.Routes.AutoMappers
{
    public class PaginationRoutesMapper : Profile
    {
        public PaginationRoutesMapper()
        {
            CreateMap<PaginationResponse<RoutesEntity>, PaginationResponseDto<RoutesDto>>().ReverseMap();
        }
    }
}

