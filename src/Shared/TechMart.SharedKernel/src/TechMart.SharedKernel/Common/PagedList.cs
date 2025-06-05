namespace TechMart.SharedKernel.Common;

/// <summary>
/// Represents a paged list of items with pagination metadata.
/// </summary>
/// <typeparam name="T">The type of items in the list.</typeparam>
public class PagedList<T>
{
    /// <summary>
    /// Gets the items in the current page.
    /// </summary>
    public IReadOnlyList<T> Items { get; }

    /// <summary>
    /// Gets the current page number (1-based).
    /// </summary>
    public int PageNumber { get; }

    /// <summary>
    /// Gets the number of items per page.
    /// </summary>
    public int PageSize { get; }

    /// <summary>
    /// Gets the total number of items across all pages.
    /// </summary>
    public int TotalCount { get; }

    /// <summary>
    /// Gets the total number of pages.
    /// </summary>
    public int TotalPages { get; }

    /// <summary>
    /// Gets a value indicating whether there is a previous page.
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;

    /// <summary>
    /// Gets a value indicating whether there is a next page.
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages;

    /// <summary>
    /// Gets the number of the first item on the current page.
    /// </summary>
    public int FirstItemOnPage => (PageNumber - 1) * PageSize + 1;

    /// <summary>
    /// Gets the number of the last item on the current page.
    /// </summary>
    public int LastItemOnPage => Math.Min(FirstItemOnPage + PageSize - 1, TotalCount);

    /// <summary>
    /// Gets a value indicating whether the current page is the first page.
    /// </summary>
    public bool IsFirstPage => PageNumber == 1;

    /// <summary>
    /// Gets a value indicating whether the current page is the last page.
    /// </summary>
    public bool IsLastPage => PageNumber == TotalPages;

    public PagedList(IEnumerable<T> items, int pageNumber, int pageSize, int totalCount)
    {
        Items = items.ToList().AsReadOnly();
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalCount = totalCount;
        TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
    }

    /// <summary>
    /// Creates an empty paged list.
    /// </summary>
    /// <param name="pageNumber">The page number.</param>
    /// <param name="pageSize">The page size.</param>
    /// <returns>An empty paged list.</returns>
    public static PagedList<T> Empty(int pageNumber = 1, int pageSize = 10) =>
        new(Array.Empty<T>(), pageNumber, pageSize, 0);

    /// <summary>
    /// Creates a paged list from a queryable source.
    /// </summary>
    /// <param name="source">The queryable source.</param>
    /// <param name="pageNumber">The page number (1-based).</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A paged list.</returns>
    public static Task<PagedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var totalCount = source.Count();
        var items = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

        return Task.FromResult(new PagedList<T>(items, pageNumber, pageSize, totalCount));
    }

    /// <summary>
    /// Creates a paged list from an enumerable source.
    /// </summary>
    /// <param name="source">The enumerable source.</param>
    /// <param name="pageNumber">The page number (1-based).</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>A paged list.</returns>
    public static PagedList<T> Create(IEnumerable<T> source, int pageNumber, int pageSize)
    {
        var enumerable = source.ToList();
        var totalCount = enumerable.Count;
        var items = enumerable.Skip((pageNumber - 1) * pageSize).Take(pageSize);

        return new PagedList<T>(items, pageNumber, pageSize, totalCount);
    }

    /// <summary>
    /// Maps the items in the paged list to a different type.
    /// </summary>
    /// <typeparam name="TResult">The target type.</typeparam>
    /// <param name="mapper">The mapping function.</param>
    /// <returns>A new paged list with mapped items.</returns>
    public PagedList<TResult> Map<TResult>(Func<T, TResult> mapper)
    {
        var mappedItems = Items.Select(mapper);
        return new PagedList<TResult>(mappedItems, PageNumber, PageSize, TotalCount);
    }
}

/// <summary>
/// Represents pagination parameters.
/// </summary>
public class PaginationParameters
{
    private int _pageNumber = 1;
    private int _pageSize = 10;

    /// <summary>
    /// Gets or sets the page number (1-based).
    /// </summary>
    public int PageNumber
    {
        get => _pageNumber;
        set => _pageNumber = value < 1 ? 1 : value;
    }

    /// <summary>
    /// Gets or sets the page size.
    /// </summary>
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value < 1 ? 10 : value > 100 ? 100 : value;
    }

    /// <summary>
    /// Gets the number of items to skip.
    /// </summary>
    public int Skip => (PageNumber - 1) * PageSize;

    /// <summary>
    /// Gets the number of items to take.
    /// </summary>
    public int Take => PageSize;
}