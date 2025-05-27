using TechMart.Auth.Domain.Users.ValueObjects;

namespace TechMart.Auth.Infrastructure.Authentication.Models;

public record PasswordResetTokenData
{
    public UserId UserId { get; init; } = null!;
    public string TokenHash { get; init; } = string.Empty; // Store hash, not raw token
    public DateTime CreatedAt { get; init; }
}
