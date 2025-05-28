namespace TechMart.Auth.Application.Common.Models;

/// <summary>
/// Parameters for pagination requests
/// </summary>
public sealed class PaginationParams
{
    private int _pageIndex = 1;
    private int _pageSize = 10;

    /// <summary>
    /// Current page index (1-based)
    /// </summary>
    public int PageIndex
    {
        get => _pageIndex;
        set => _pageIndex = value < 1 ? 1 : value;
    }

    /// <summary>
    /// Number of items per page
    /// </summary>
    public int PageSize
    {
        get => _pageSize;
        set =>
            _pageSize = value switch
            {
                < 1 => 10,
                > MaxPageSize => MaxPageSize,
                _ => value,
            };
    }

    /// <summary>
    /// Search term for filtering
    /// </summary>
    public string? SearchTerm { get; set; }

    /// <summary>
    /// Field to sort by
    /// </summary>
    public string? SortBy { get; set; }

    /// <summary>
    /// Sort direction (true for ascending, false for descending)
    /// </summary>
    public bool SortAscending { get; set; } = true;

    /// <summary>
    /// Maximum allowed page size
    /// </summary>
    public const int MaxPageSize = 100;

    /// <summary>
    /// Skip count for database queries
    /// </summary>
    public int Skip => (PageIndex - 1) * PageSize;

    /// <summary>
    /// Take count for database queries
    /// </summary>
    public int Take => PageSize;

    /// <summary>
    /// Whether search term is provided and not empty
    /// </summary>
    public bool HasSearchTerm => !string.IsNullOrWhiteSpace(SearchTerm);

    /// <summary>
    /// Whether sorting is specified
    /// </summary>
    public bool HasSorting => !string.IsNullOrWhiteSpace(SortBy);

    /// <summary>
    /// Creates default pagination parameters
    /// </summary>
    public static PaginationParams Default() => new();

    /// <summary>
    /// Creates pagination parameters with specified page and size
    /// </summary>
    public static PaginationParams Create(int pageIndex, int pageSize)
    {
        return new PaginationParams { PageIndex = pageIndex, PageSize = pageSize };
    }

    /// <summary>
    /// Creates pagination parameters with search
    /// </summary>
    public static PaginationParams WithSearch(int pageIndex, int pageSize, string? searchTerm)
    {
        return new PaginationParams
        {
            PageIndex = pageIndex,
            PageSize = pageSize,
            SearchTerm = searchTerm,
        };
    }

    /// <summary>
    /// Creates pagination parameters with search and sorting
    /// </summary>
    public static PaginationParams WithSearchAndSort(
        int pageIndex,
        int pageSize,
        string? searchTerm,
        string? sortBy,
        bool sortAscending = true
    )
    {
        return new PaginationParams
        {
            PageIndex = pageIndex,
            PageSize = pageSize,
            SearchTerm = searchTerm,
            SortBy = sortBy,
            SortAscending = sortAscending,
        };
    }
}
