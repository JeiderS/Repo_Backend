using System.Collections.Generic;

namespace Fleet.Domain.Common.Pagination
{
    public class PaginationResponse<T>
    {
        public List<T> Data { get; set; } = new List<T>();
        public int TotalPages { get; set; }
        public int TotalRows { get; set; }
    }
}
