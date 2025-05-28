namespace TechMart.Auth.Application.Common.Models;

/// <summary>
/// Represents a paginated result with data and pagination metadata
/// </summary>
/// <typeparam name="T">Type of items in the result</typeparam>
public sealed class PaginatedResult<T>
{
    public PaginatedResult(IReadOnlyList<T> items, int totalCount, int pageIndex, int pageSize)
    {
        Items = items;
        TotalCount = totalCount;
        PageIndex = pageIndex;
        PageSize = pageSize;
    }

    /// <summary>
    /// The items for the current page
    /// </summary>
    public IReadOnlyList<T> Items { get; }

    /// <summary>
    /// Total number of items across all pages
    /// </summary>
    public int TotalCount { get; }

    /// <summary>
    /// Current page index (1-based)
    /// </summary>
    public int PageIndex { get; }

    /// <summary>
    /// Number of items per page
    /// </summary>
    public int PageSize { get; }

    /// <summary>
    /// Total number of pages
    /// </summary>
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    /// <summary>
    /// Whether there is a next page
    /// </summary>
    public bool HasNextPage => PageIndex < TotalPages;

    /// <summary>
    /// Whether there is a previous page
    /// </summary>
    public bool HasPreviousPage => PageIndex > 1;

    /// <summary>
    /// Number of items in the current page
    /// </summary>
    public int ItemsCount => Items.Count;

    /// <summary>
    /// Creates an empty paginated result
    /// </summary>
    public static PaginatedResult<T> Empty(int pageIndex = 1, int pageSize = 10)
    {
        return new PaginatedResult<T>(Array.Empty<T>(), totalCount: 0, pageIndex, pageSize);
    }

    /// <summary>
    /// Creates a paginated result from a collection
    /// </summary>
    public static PaginatedResult<T> Create(IEnumerable<T> source, int pageIndex, int pageSize)
    {
        var items = source.ToList();
        var totalCount = items.Count;

        var pagedItems = items
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToList()
            .AsReadOnly();

        return new PaginatedResult<T>(pagedItems, totalCount, pageIndex, pageSize);
    }
}
