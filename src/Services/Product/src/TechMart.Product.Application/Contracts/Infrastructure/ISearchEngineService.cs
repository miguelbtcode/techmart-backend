namespace TechMart.Product.Application.Contracts.Infrastructure;

/// <summary>
/// Service for search engine operations.
/// </summary>
public interface ISearchEngineService
{
    /// <summary>
    /// Indexes a product for search.
    /// </summary>
    /// <param name="productData">The product data to index.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task IndexProductAsync(object productData, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a product from the search index.
    /// </summary>
    /// <param name="productId">The product ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task RemoveProductFromIndexAsync(Guid productId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches for products.
    /// </summary>
    /// <param name="query">The search query.</param>
    /// <param name="filters">Additional filters.</param>
    /// <param name="pageNumber">The page number.</param>
    /// <param name="pageSize">The page size.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Search results.</returns>
    Task<SearchResults> SearchProductsAsync(string query, Dictionary<string, object>? filters = null, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets search suggestions.
    /// </summary>
    /// <param name="query">The partial query.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of suggestions.</returns>
    Task<List<string>> GetSuggestionsAsync(string query, CancellationToken cancellationToken = default);
}

/// <summary>
/// Represents search results.
/// </summary>
public class SearchResults
{
    public List<object> Items { get; set; } = new();
    public long TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public Dictionary<string, object> Aggregations { get; set; } = new();
}