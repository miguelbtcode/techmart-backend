using TechMart.Auth.Domain.Users.ValueObjects;

namespace TechMart.Auth.Application.Contracts.Infrastructure;

/// <summary>
/// Service for password reset token management
/// </summary>
public interface IPasswordResetService
{
    /// <summary>
    /// Generates a password reset token for a user
    /// </summary>
    Task<string> GenerateResetTokenAsync(
        UserId userId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Validates a password reset token and returns the user ID
    /// </summary>
    Task<UserId?> ValidateResetTokenAsync(
        string email,
        string token,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Invalidates a specific reset token
    /// </summary>
    Task InvalidateTokenAsync(string token, CancellationToken cancellationToken = default);

    /// <summary>
    /// Invalidates all reset tokens for a user
    /// </summary>
    Task InvalidateAllTokensAsync(UserId userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a token is valid without consuming it
    /// </summary>
    Task<bool> IsTokenValidAsync(
        string email,
        string token,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Gets token expiration time
    /// </summary>
    Task<DateTime?> GetTokenExpirationAsync(
        string token,
        CancellationToken cancellationToken = default
    );
}
