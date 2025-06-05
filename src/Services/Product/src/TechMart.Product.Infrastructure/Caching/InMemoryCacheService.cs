using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using TechMart.Product.Application.Contracts.Caching;

namespace TechMart.Product.Infrastructure.Caching;

/// <summary>
/// In-memory cache service implementation as fallback when Redis is not available.
/// </summary>
public class InMemoryCacheService : ICacheService
{
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<InMemoryCacheService> _logger;
    private readonly HashSet<string> _cacheKeys;
    private readonly object _lockObject = new();

    public InMemoryCacheService(
        IMemoryCache memoryCache,
        ILogger<InMemoryCacheService> logger)
    {
        _memoryCache = memoryCache;
        _logger = logger;
        _cacheKeys = new HashSet<string>();
    }

    public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            if (_memoryCache.TryGetValue(key, out var cachedValue))
            {
                if (cachedValue is string jsonString)
                {
                    var result = JsonSerializer.Deserialize<T>(jsonString);
                    _logger.LogDebug("Cache hit for key: {Key}", key);
                    return Task.FromResult(result);
                }
                
                if (cachedValue is T directValue)
                {
                    _logger.LogDebug("Cache hit for key: {Key}", key);
                    return Task.FromResult<T?>(directValue);
                }
            }

            _logger.LogDebug("Cache miss for key: {Key}", key);
            return Task.FromResult<T?>(null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cached value for key: {Key}", key);
            return Task.FromResult<T?>(null);
        }
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? slidingExpiration = null, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            var options = new MemoryCacheEntryOptions();
            
            if (slidingExpiration.HasValue)
            {
                options.SlidingExpiration = slidingExpiration.Value;
            }
            else
            {
                options.SlidingExpiration = TimeSpan.FromMinutes(30); // Default expiration
            }

            // Set absolute expiration to prevent memory leaks
            options.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(4);

            // Add eviction callback to remove from tracking
            options.RegisterPostEvictionCallback(OnCacheEntryEvicted);

            var jsonString = JsonSerializer.Serialize(value);
            _memoryCache.Set(key, jsonString, options);

            // Track cache keys for pattern-based removal
            lock (_lockObject)
            {
                _cacheKeys.Add(key);
            }

            _logger.LogDebug("Cached value for key: {Key} with expiration: {Expiration}", 
                key, slidingExpiration?.ToString() ?? "30 minutes");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting cached value for key: {Key}", key);
        }

        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            _memoryCache.Remove(key);
            
            lock (_lockObject)
            {
                _cacheKeys.Remove(key);
            }

            _logger.LogDebug("Removed cached value for key: {Key}", key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing cached value for key: {Key}", key);
        }

        return Task.CompletedTask;
    }

    public Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default)
    {
        try
        {
            // Convert cache pattern to regex pattern
            var regexPattern = ConvertPatternToRegex(pattern);
            var regex = new Regex(regexPattern, RegexOptions.IgnoreCase);

            var keysToRemove = new List<string>();

            lock (_lockObject)
            {
                keysToRemove.AddRange(_cacheKeys.Where(key => regex.IsMatch(key)));
            }

            foreach (var key in keysToRemove)
            {
                _memoryCache.Remove(key);
                
                lock (_lockObject)
                {
                    _cacheKeys.Remove(key);
                }
            }

            _logger.LogDebug("Removed {Count} cached values matching pattern: {Pattern}", 
                keysToRemove.Count, pattern);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing cached values by pattern: {Pattern}", pattern);
        }

        return Task.CompletedTask;
    }

    private void OnCacheEntryEvicted(object key, object value, EvictionReason reason, object state)
    {
        if (key is string keyString)
        {
            lock (_lockObject)
            {
                _cacheKeys.Remove(keyString);
            }
            
            _logger.LogDebug("Cache entry evicted: {Key}, Reason: {Reason}", keyString, reason);
        }
    }

    private static string ConvertPatternToRegex(string pattern)
    {
        // Convert wildcard pattern to regex
        // Replace * with .* and escape other regex special characters
        var escaped = Regex.Escape(pattern);
        var regexPattern = escaped.Replace("\\*", ".*");
        return $"^{regexPattern}$";
    }
}