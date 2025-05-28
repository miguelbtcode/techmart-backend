using TechMart.Auth.Application.Contracts.Authentication;
using TechMart.Auth.Domain.Users.Entities;

namespace TechMart.Auth.Application.Contracts.Infrastructure;

/// <summary>
/// Service for refresh token management
/// </summary>
public interface IRefreshTokenService
{
    /// <summary>
    /// Generates a new refresh token for a user
    /// </summary>
    Task<string> GenerateRefreshTokenAsync(
        User user,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Validates a refresh token and returns validation result
    /// </summary>
    Task<TokenValidationResult> ValidateRefreshTokenAsync(
        string refreshToken,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Revokes a specific refresh token
    /// </summary>
    Task RevokeRefreshTokenAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Revokes all refresh tokens for a user (logout from all devices)
    /// </summary>
    Task RevokeAllRefreshTokensAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all active refresh tokens for a user (for security dashboard)
    /// </summary>
    Task<IEnumerable<RefreshTokenInfo>> GetActiveTokensAsync(
        Guid userId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Cleans up expired refresh tokens
    /// </summary>
    Task CleanupExpiredTokensAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Information about a refresh token
/// </summary>
public sealed record RefreshTokenInfo(
    string TokenId,
    Guid UserId,
    DateTime CreatedAt,
    DateTime ExpiresAt,
    string? DeviceInfo = null,
    string? IpAddress = null
);
