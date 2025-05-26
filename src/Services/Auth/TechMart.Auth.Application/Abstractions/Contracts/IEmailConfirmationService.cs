namespace TechMart.Auth.Application.Abstractions.Contracts;

public interface IEmailConfirmationService
{
    Task<string> GenerateTokenAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> ValidateTokenAsync(
        string email,
        string token,
        CancellationToken cancellationToken = default
    );
}
