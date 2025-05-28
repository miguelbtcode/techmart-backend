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
    /// Validates an email confirmation token
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
    /// Checks if a token exists and is valid (without consuming it)
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
        string email,
        string token,
        CancellationToken cancellationToken = default
    );
}
