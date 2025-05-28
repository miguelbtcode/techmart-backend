using System.Text.Json;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using TechMart.Auth.Application.Contracts.Infrastructure;

namespace TechMart.Auth.Infrastructure.Caching;

public sealed class RedisCacheService : ICacheService
{
    private readonly IDatabase _database;
    private readonly IConnectionMultiplexer _redis;
    private readonly ILogger<RedisCacheService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public RedisCacheService(IConnectionMultiplexer redis, ILogger<RedisCacheService> logger)
    {
        _redis = redis;
        _database = redis.GetDatabase();
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false,
        };
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var value = await _database.StringGetAsync(key);

            if (!value.HasValue)
                return default;

            return JsonSerializer.Deserialize<T>(value!, _jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cache key {Key}", key);
            return default;
        }
    }

    public async Task SetAsync<T>(
        string key,
        T value,
        TimeSpan? expiry = null,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var serializedValue = JsonSerializer.Serialize(value, _jsonOptions);
            await _database.StringSetAsync(key, serializedValue, expiry);

            _logger.LogDebug("Cache key {Key} set with expiry {Expiry}", key, expiry);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting cache key {Key}", key);
        }
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            await _database.KeyDeleteAsync(key);
            _logger.LogDebug("Cache key {Key} removed", key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing cache key {Key}", key);
        }
    }

    public async Task RemoveByPatternAsync(
        string pattern,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var server = _redis.GetServer(_redis.GetEndPoints().First());
            var keys = server.Keys(pattern: pattern);

            foreach (var key in keys)
            {
                await _database.KeyDeleteAsync(key);
            }

            _logger.LogDebug("Cache keys matching pattern {Pattern} removed", pattern);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing cache keys by pattern {Pattern}", pattern);
        }
    }

    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _database.KeyExistsAsync(key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking cache key existence {Key}", key);
            return false;
        }
    }

    public async Task<IEnumerable<string>> GetKeysByPatternAsync(
        string pattern,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var server = _redis.GetServer(_redis.GetEndPoints().First());
            var keys = server.Keys(pattern: pattern).Select(k => k.ToString());

            return await Task.FromResult(keys);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting keys by pattern {Pattern}", pattern);
            return Enumerable.Empty<string>();
        }
    }
}
