namespace TechMart.Auth.Application.Contracts.Infrastructure;

/// <summary>
/// Service for caching operations
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// Gets a cached value by key
    /// </summary>
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets a value in cache with optional expiration
    /// </summary>
    Task SetAsync<T>(
        string key,
        T value,
        TimeSpan? expiry = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Removes a cached value by key
    /// </summary>
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes multiple cached values by pattern
    /// </summary>
    Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a key exists in cache
    /// </summary>
    Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all keys matching a pattern
    /// </summary>
    Task<IEnumerable<string>> GetKeysByPatternAsync(
        string pattern,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Sets multiple values in cache atomically
    /// </summary>
    Task SetManyAsync<T>(
        IDictionary<string, T> keyValuePairs,
        TimeSpan? expiry = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Gets multiple values from cache
    /// </summary>
    Task<IDictionary<string, T?>> GetManyAsync<T>(
        IEnumerable<string> keys,
        CancellationToken cancellationToken = default
    );
}
