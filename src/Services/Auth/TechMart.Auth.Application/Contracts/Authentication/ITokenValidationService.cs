namespace TechMart.Auth.Application.Contracts.Authentication;

/// <summary>
/// Service for comprehensive token validation
/// </summary>
public interface ITokenValidationService
{
    /// <summary>
    /// Validates an access token with full security checks
    /// </summary>
    Task<TokenValidationResult> ValidateAccessTokenAsync(
        string accessToken,
        bool validateLifetime = true,
        bool validateIssuer = true,
        bool validateAudience = true,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Validates a refresh token
    /// </summary>
    Task<TokenValidationResult> ValidateRefreshTokenAsync(
        string refreshToken,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Validates an email confirmation token
    /// </summary>
    Task<TokenValidationResult> ValidateEmailConfirmationTokenAsync(
        string email,
        string token,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Validates a password reset token
    /// </summary>
    Task<TokenValidationResult> ValidatePasswordResetTokenAsync(
        string email,
        string token,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Checks if a token is blacklisted or revoked
    /// </summary>
    Task<bool> IsTokenRevokedAsync(string tokenId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Revokes a token (adds to blacklist)
    /// </summary>
    Task RevokeTokenAsync(string tokenId, CancellationToken cancellationToken = default);
}
