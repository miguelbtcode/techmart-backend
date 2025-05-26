using TechMart.Auth.Domain.Users.ValueObjects;

namespace TechMart.Auth.Application.Abstractions.Contracts;

public interface IPasswordResetService
{
    Task<string> GenerateResetTokenAsync(
        UserId userId,
        CancellationToken cancellationToken = default
    );
    Task<UserId?> ValidateResetTokenAsync(
        string email,
        string token,
        CancellationToken cancellationToken = default
    );
    Task InvalidateTokenAsync(string token, CancellationToken cancellationToken = default);
}
