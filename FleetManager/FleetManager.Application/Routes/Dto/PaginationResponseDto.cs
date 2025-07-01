

namespace FleetManager.Application.Routes.Dto
{
    public class PaginationResponseDto<T>
    {
        public List<T> Data { get; set; } = new List<T>();
        public int TotalPages { get; set; }
        public int TotalRows { get; set; }
    }
}

