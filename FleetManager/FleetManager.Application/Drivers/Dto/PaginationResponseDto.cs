using System.Collections.Generic;

namespace FleetManager.Application.Drivers.Dto
{
    public class PaginationResponseDto<T>
    {
        public List<T> Data { get; set; }
        public int TotalPages { get; set; }
        public int TotalRows { get; set; }
    }
}

