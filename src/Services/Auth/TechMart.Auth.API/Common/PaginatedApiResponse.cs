namespace TechMart.Auth.API.Common;

public class PaginatedApiResponse<T> : ApiResponse<IEnumerable<T>>
{
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public bool HasNextPage { get; set; }
    public bool HasPreviousPage { get; set; }
}
