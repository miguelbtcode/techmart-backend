using TechMart.Auth.Application.Abstractions.Authentication;
using TechMart.Auth.Domain.Users.Entities;

namespace TechMart.Auth.Application.Abstractions.Contracts;

/// <summary>
/// Service for managing refresh tokens
/// </summary>
public interface IRefreshTokenService
{
    /// <summary>
    /// Generates a new refresh token for a user
    /// </summary>
    /// <param name="user">User to generate refresh token for</param>
    /// <returns>Refresh token string</returns>
    Task<string> GenerateRefreshTokenAsync(User user);

    /// <summary>
    /// Validates a refresh token
    /// </summary>
    /// <param name="refreshToken">Refresh token to validate</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Token validation result</returns>
    Task<TokenValidationResult> ValidateRefreshTokenAsync(
        string refreshToken,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Revokes a refresh token for a specific user
    /// </summary>
    /// <param name="userId">User ID whose refresh token to revoke</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task RevokeRefreshTokenAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Revokes all refresh tokens for a user (logout from all devices)
    /// </summary>
    /// <param name="userId">User ID whose refresh tokens to revoke</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task RevokeAllRefreshTokensAsync(Guid userId, CancellationToken cancellationToken = default);
}
