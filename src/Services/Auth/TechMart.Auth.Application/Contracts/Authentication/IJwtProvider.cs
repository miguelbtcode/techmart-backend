using TechMart.Auth.Domain.Users.Entities;

namespace TechMart.Auth.Application.Contracts.Authentication;

/// <summary>
/// JWT token provider for authentication
/// </summary>
public interface IJwtProvider
{
    /// <summary>
    /// Generates a JWT access token for the user
    /// </summary>
    string GenerateToken(User user);

    /// <summary>
    /// Validates an access token and returns validation result
    /// </summary>
    Task<TokenValidationResult> ValidateAccessTokenAsync(
        string accessToken,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Gets claims from a valid token without full validation
    /// </summary>
    Task<IEnumerable<System.Security.Claims.Claim>> GetClaimsFromTokenAsync(
        string accessToken,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Gets user ID from a valid token
    /// </summary>
    Task<Guid?> GetUserIdFromTokenAsync(
        string accessToken,
        CancellationToken cancellationToken = default
    );
}

/// <summary>
/// Result of token validation
/// </summary>
public sealed record TokenValidationResult(
    bool IsValid,
    Guid? UserId,
    DateTime? ExpiresAt,
    IEnumerable<string>? Roles = null,
    string? ErrorMessage = null
);
