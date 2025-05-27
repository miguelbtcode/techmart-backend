using TechMart.Auth.Domain.Users.Entities;

namespace TechMart.Auth.Application.Abstractions.Authentication;

public interface IJwtProvider
{
    string GenerateToken(User user);
    Task<TokenValidationResult> ValidateAccessTokenAsync(
        string accessToken,
        CancellationToken cancellationToken = default
    );
}

public sealed record TokenValidationResult(
    bool IsValid,
    Guid? UserId,
    DateTime? ExpiresAt,
    IEnumerable<string>? Roles = null
);
