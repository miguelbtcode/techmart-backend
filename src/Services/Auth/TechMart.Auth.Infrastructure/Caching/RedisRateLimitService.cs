using Microsoft.Extensions.Logging;
using TechMart.Auth.Application.Contracts.Infrastructure;

namespace TechMart.Auth.Infrastructure.Caching;

/// <summary>
/// Redis-based implementation of rate limiting service
/// Provides robust rate limiting with configurable windows and blocking
/// </summary>
public sealed class RedisRateLimitService : IRateLimitService
{
    private readonly ICacheService _cache;
    private readonly ILogger<RedisRateLimitService> _logger;

    // Default configuration
    private static readonly TimeSpan DefaultWindow = TimeSpan.FromMinutes(15);
    private static readonly TimeSpan DefaultBlockDuration = TimeSpan.FromHours(1);
    private const int DefaultMaxAttempts = 5;

    public RedisRateLimitService(ICacheService cache, ILogger<RedisRateLimitService> logger)
    {
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> IsAllowedAsync(
        string identifier,
        int maxAttempts = DefaultMaxAttempts,
        TimeSpan? window = null,
        CancellationToken cancellationToken = default
    )
    {
        if (string.IsNullOrWhiteSpace(identifier))
        {
            _logger.LogWarning("Rate limit check called with null or empty identifier");
            return false;
        }

        try
        {
            // Check if identifier is blocked first
            if (await IsBlockedAsync(identifier, cancellationToken))
            {
                _logger.LogWarning(
                    "Rate limit check failed - identifier {Identifier} is blocked",
                    identifier
                );
                return false;
            }

            var key = CacheKeys.LoginAttempts(identifier);
            var currentAttempts = await _cache.GetAsync<int?>(key, cancellationToken) ?? 0;

            var isAllowed = currentAttempts < maxAttempts;

            _logger.LogDebug(
                "Rate limit check for {Identifier}: {CurrentAttempts}/{MaxAttempts} - {Result}",
                identifier,
                currentAttempts,
                maxAttempts,
                isAllowed ? "ALLOWED" : "BLOCKED"
            );

            return isAllowed;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to check rate limit for identifier {Identifier}",
                identifier
            );
            // In case of error, be conservative and allow the request
            // This prevents cache failures from breaking authentication completely
            return true;
        }
    }

    public async Task IncrementAsync(
        string identifier,
        TimeSpan? window = null,
        CancellationToken cancellationToken = default
    )
    {
        if (string.IsNullOrWhiteSpace(identifier))
        {
            _logger.LogWarning("Rate limit increment called with null or empty identifier");
            return;
        }

        try
        {
            var key = CacheKeys.LoginAttempts(identifier);
            var windowToUse = window ?? DefaultWindow;

            var currentAttempts = await _cache.GetAsync<int?>(key, cancellationToken) ?? 0;
            var newAttempts = currentAttempts + 1;

            await _cache.SetAsync(key, newAttempts, windowToUse, cancellationToken);

            _logger.LogDebug(
                "Rate limit incremented for {Identifier}: {Attempts} attempts (window: {Window})",
                identifier,
                newAttempts,
                windowToUse
            );

            // Auto-block if attempts exceed threshold
            if (newAttempts >= DefaultMaxAttempts * 2) // Double the normal limit triggers auto-block
            {
                await BlockAsync(identifier, DefaultBlockDuration, cancellationToken);
                _logger.LogWarning(
                    "Auto-blocking identifier {Identifier} due to excessive attempts: {Attempts}",
                    identifier,
                    newAttempts
                );
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to increment rate limit for identifier {Identifier}",
                identifier
            );
            throw new InvalidOperationException("Rate limit increment failed", ex);
        }
    }

    public async Task ResetAsync(string identifier, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(identifier))
        {
            _logger.LogWarning("Rate limit reset called with null or empty identifier");
            return;
        }

        try
        {
            var key = CacheKeys.LoginAttempts(identifier);
            await _cache.RemoveAsync(key, cancellationToken);

            _logger.LogDebug("Rate limit reset for identifier {Identifier}", identifier);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to reset rate limit for identifier {Identifier}",
                identifier
            );
            throw new InvalidOperationException("Rate limit reset failed", ex);
        }
    }

    public async Task<int> GetAttemptCountAsync(
        string identifier,
        CancellationToken cancellationToken = default
    )
    {
        if (string.IsNullOrWhiteSpace(identifier))
        {
            _logger.LogWarning("Get attempt count called with null or empty identifier");
            return 0;
        }

        try
        {
            var key = CacheKeys.LoginAttempts(identifier);
            var attempts = await _cache.GetAsync<int?>(key, cancellationToken) ?? 0;

            _logger.LogDebug(
                "Current attempt count for {Identifier}: {Attempts}",
                identifier,
                attempts
            );
            return attempts;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to get attempt count for identifier {Identifier}",
                identifier
            );
            return 0;
        }
    }

    public async Task<TimeSpan?> GetTimeUntilResetAsync(
        string identifier,
        CancellationToken cancellationToken = default
    )
    {
        if (string.IsNullOrWhiteSpace(identifier))
        {
            _logger.LogWarning("Get time until reset called with null or empty identifier");
            return null;
        }

        try
        {
            var key = CacheKeys.LoginAttempts(identifier);

            // This is a limitation - Redis doesn't easily expose TTL through our ICacheService
            // In a real implementation, you might need to extend ICacheService to include TTL operations
            // For now, we'll return null to indicate the feature isn't available
            _logger.LogDebug(
                "Time until reset requested for {Identifier} - feature not available through current cache abstraction",
                identifier
            );
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to get time until reset for identifier {Identifier}",
                identifier
            );
            return null;
        }
    }

    public async Task BlockAsync(
        string identifier,
        TimeSpan duration,
        CancellationToken cancellationToken = default
    )
    {
        if (string.IsNullOrWhiteSpace(identifier))
        {
            _logger.LogWarning("Block called with null or empty identifier");
            return;
        }

        try
        {
            var blockKey = GetBlockKey(identifier);
            var blockData = new RateLimitBlockData
            {
                Identifier = identifier,
                BlockedAt = DateTime.UtcNow,
                BlockDuration = duration,
                Reason = "Rate limit exceeded",
            };

            await _cache.SetAsync(blockKey, blockData, duration, cancellationToken);

            _logger.LogWarning(
                "Identifier {Identifier} blocked for {Duration}",
                identifier,
                duration
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to block identifier {Identifier}", identifier);
            throw new InvalidOperationException("Blocking failed", ex);
        }
    }

    public async Task UnblockAsync(string identifier, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(identifier))
        {
            _logger.LogWarning("Unblock called with null or empty identifier");
            return;
        }

        try
        {
            var blockKey = GetBlockKey(identifier);
            await _cache.RemoveAsync(blockKey, cancellationToken);

            // Also reset attempt count when unblocking
            await ResetAsync(identifier, cancellationToken);

            _logger.LogInformation("Identifier {Identifier} unblocked", identifier);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to unblock identifier {Identifier}", identifier);
            throw new InvalidOperationException("Unblocking failed", ex);
        }
    }

    public async Task<bool> IsBlockedAsync(
        string identifier,
        CancellationToken cancellationToken = default
    )
    {
        if (string.IsNullOrWhiteSpace(identifier))
        {
            _logger.LogWarning("Is blocked check called with null or empty identifier");
            return false;
        }

        try
        {
            var blockKey = GetBlockKey(identifier);
            var blockData = await _cache.GetAsync<RateLimitBlockData>(blockKey, cancellationToken);

            var isBlocked = blockData != null;

            if (isBlocked)
            {
                _logger.LogDebug(
                    "Identifier {Identifier} is blocked until {BlockedAt} + {Duration}",
                    identifier,
                    blockData!.BlockedAt,
                    blockData.BlockDuration
                );
            }

            return isBlocked;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to check if identifier {Identifier} is blocked",
                identifier
            );
            // In case of error, assume not blocked to avoid breaking authentication
            return false;
        }
    }

    /// <summary>
    /// Get block-specific cache key
    /// </summary>
    private static string GetBlockKey(string identifier)
    {
        return CacheKeys.BlockedIdentifier(identifier);
    }

    /// <summary>
    /// Clean up expired rate limit entries (optional background operation)
    /// </summary>
    public async Task CleanupExpiredEntriesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            // Redis handles TTL automatically, but we could implement cleanup logic here
            // if we needed to maintain statistics or perform other cleanup operations
            _logger.LogDebug("Rate limit cleanup completed (handled by Redis TTL)");
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to cleanup expired rate limit entries");
        }
    }

    /// <summary>
    /// Get rate limit statistics for monitoring
    /// </summary>
    public async Task<RateLimitStatistics> GetStatisticsAsync(
        string? identifierPattern = null,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            // This would require extending ICacheService to support pattern-based operations
            // For now, return basic statistics
            var stats = new RateLimitStatistics
            {
                TotalIdentifiersTracked = 0, // Would need pattern search
                TotalBlockedIdentifiers = 0, // Would need pattern search
                LastCleanupTime = DateTime.UtcNow,
                StatisticsGeneratedAt = DateTime.UtcNow,
            };

            _logger.LogDebug("Rate limit statistics generated");
            return stats;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get rate limit statistics");
            throw new InvalidOperationException("Statistics generation failed", ex);
        }
    }
}

/// <summary>
/// Data stored when an identifier is blocked
/// </summary>
internal sealed record RateLimitBlockData
{
    public string Identifier { get; init; } = string.Empty;
    public DateTime BlockedAt { get; init; }
    public TimeSpan BlockDuration { get; init; }
    public string Reason { get; init; } = string.Empty;
}

/// <summary>
/// Rate limit statistics for monitoring
/// </summary>
public sealed record RateLimitStatistics
{
    public int TotalIdentifiersTracked { get; init; }
    public int TotalBlockedIdentifiers { get; init; }
    public DateTime LastCleanupTime { get; init; }
    public DateTime StatisticsGeneratedAt { get; init; }
}
