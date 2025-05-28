namespace TechMart.Auth.Application.Contracts.Infrastructure;

/// <summary>
/// Service for rate limiting operations
/// </summary>
public interface IRateLimitService
{
    /// <summary>
    /// Checks if an operation is allowed for the given identifier
    /// </summary>
    Task<bool> IsAllowedAsync(
        string identifier,
        int maxAttempts = 5,
        TimeSpan? window = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Increments the attempt counter for an identifier
    /// </summary>
    Task IncrementAsync(
        string identifier,
        TimeSpan? window = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Resets the attempt counter for an identifier
    /// </summary>
    Task ResetAsync(string identifier, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the current attempt count for an identifier
    /// </summary>
    Task<int> GetAttemptCountAsync(
        string identifier,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Gets the time until the rate limit resets
    /// </summary>
    Task<TimeSpan?> GetTimeUntilResetAsync(
        string identifier,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Blocks an identifier for a specific duration
    /// </summary>
    Task BlockAsync(
        string identifier,
        TimeSpan duration,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Unblocks an identifier
    /// </summary>
    Task UnblockAsync(string identifier, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if an identifier is currently blocked
    /// </summary>
    Task<bool> IsBlockedAsync(string identifier, CancellationToken cancellationToken = default);
}
