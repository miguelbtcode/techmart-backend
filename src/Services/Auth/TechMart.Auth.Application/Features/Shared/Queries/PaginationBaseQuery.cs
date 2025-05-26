namespace TechMart.Auth.Application.Features.Shared.Queries;

public abstract record PaginationBaseQuery
{
    public string? SearchTerm { get; set; }
    public string? SortBy { get; set; }
    public int PageIndex { get; set; } = 1;
    private int _pageSize = 3;
    private const int MaxPageSize = 50;
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
    }
}
