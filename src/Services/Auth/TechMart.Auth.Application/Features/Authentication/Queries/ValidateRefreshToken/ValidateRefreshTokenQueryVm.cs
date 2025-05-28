namespace TechMart.Auth.Application.Features.Authentication.Queries.ValidateRefreshToken;

public sealed record ValidateRefreshTokenQueryVm(
    bool IsValid,
    Guid? UserId,
    DateTime? ExpiresAt,
    IEnumerable<string>? Roles = null,
    string? ErrorMessage = null
);
