namespace TechMart.Auth.Application.Contracts.Infrastructure;

/// <summary>
/// Service for email confirmation token management
/// </summary>
public interface IEmailConfirmationService
{
    /// <summary>
    /// Generates a new email confirmation token
    /// </summary>
    Task<string> GenerateTokenAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates an email confirmation token (one-time use)
    /// </summary>
    Task<bool> ValidateTokenAsync(
        string email,
        string token,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Invalidates all email confirmation tokens for an email
    /// </summary>
    Task InvalidateTokensAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if an email confirmation token is valid without consuming it
    /// </summary>
    Task<bool> IsEmailConfirmationTokenValidAsync(
        string email,
        string token,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Gets email confirmation token expiration time
    /// </summary>
    Task<DateTime?> GetEmailConfirmationTokenExpirationAsync(
        string email,
        string token,
        CancellationToken cancellationToken = default
    );
}
