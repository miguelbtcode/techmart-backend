using TechMart.Auth.Domain.Primitives;

namespace TechMart.Auth.Application.Contracts.Services;

/// <summary>
/// Application service for security-related operations
/// </summary>
public interface ISecurityService
{
    /// <summary>
    /// Validates if a user can perform an action on a resource
    /// </summary>
    Task<Result<bool>> CanAccessResourceAsync(
        Guid userId,
        string resourceType,
        string resourceId,
        string action,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Logs a security event
    /// </summary>
    Task LogSecurityEventAsync(
        string eventType,
        Guid? userId = null,
        string? details = null,
        string? ipAddress = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Validates password strength
    /// </summary>
    Task<Result<PasswordStrengthResult>> ValidatePasswordStrengthAsync(
        string password,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Checks if an IP address is blocked
    /// </summary>
    Task<Result<bool>> IsIpBlockedAsync(
        string ipAddress,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Blocks an IP address
    /// </summary>
    Task<Result> BlockIpAddressAsync(
        string ipAddress,
        TimeSpan? duration = null,
        string? reason = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Unblocks an IP address
    /// </summary>
    Task<Result> UnblockIpAddressAsync(
        string ipAddress,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Gets failed login attempts for a user
    /// </summary>
    Task<Result<int>> GetFailedLoginAttemptsAsync(
        string email,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Resets failed login attempts for a user
    /// </summary>
    Task<Result> ResetFailedLoginAttemptsAsync(
        string email,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Checks if a user account is locked due to failed attempts
    /// </summary>
    Task<Result<bool>> IsAccountLockedAsync(
        string email,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Unlocks a user account
    /// </summary>
    Task<Result> UnlockAccountAsync(string email, CancellationToken cancellationToken = default);
}

/// <summary>
/// Result of password strength validation
/// </summary>
public sealed record PasswordStrengthResult(
    bool IsValid,
    int Score,
    IEnumerable<string> Suggestions = null!
);
