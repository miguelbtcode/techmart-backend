using Microsoft.Extensions.Logging;
using TechMart.Auth.Application.Abstractions.Caching;
using TechMart.Auth.Application.Abstractions.Contracts;

namespace TechMart.Auth.Infrastructure.Caching;

public class RedisRateLimitService : IRateLimitService
{
    private readonly ICacheService _cache;
    private readonly ILogger<RedisRateLimitService> _logger;

    public RedisRateLimitService(ICacheService cache, ILogger<RedisRateLimitService> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<bool> IsAllowedAsync(
        string identifier,
        int maxAttempts = 5,
        TimeSpan? window = null
    )
    {
        var key = CacheKeys.LoginAttempts(identifier);
        var attempts = await _cache.GetAsync<int>(key);

        return attempts < maxAttempts;
    }

    public async Task IncrementAsync(string identifier, TimeSpan? window = null)
    {
        var key = CacheKeys.LoginAttempts(identifier);
        var attempts = await _cache.GetAsync<int>(key);

        await _cache.SetAsync(key, attempts + 1, window ?? TimeSpan.FromMinutes(15));
    }

    public async Task ResetAsync(string identifier)
    {
        var key = CacheKeys.LoginAttempts(identifier);
        await _cache.RemoveAsync(key);
    }
}
