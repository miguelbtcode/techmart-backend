namespace TechMart.Auth.Infrastructure.Authentication.Models;

public record RefreshTokenData
{
    public Guid UserId { get; init; }
    public string Email { get; init; } = string.Empty;
    public string TokenHash { get; init; } = string.Empty; // Store hash, not raw token
    public DateTime CreatedAt { get; init; }
}
