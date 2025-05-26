namespace TechMart.Auth.Application.Abstractions.Contracts;

public interface IEmailService
{
    Task SendWelcomeEmailAsync(
        string email,
        string firstName,
        CancellationToken cancellationToken = default
    );
    Task SendEmailConfirmationAsync(
        string email,
        string firstName,
        string confirmationToken,
        CancellationToken cancellationToken = default
    );
    Task SendPasswordResetEmailAsync(
        string email,
        string firstName,
        string resetToken,
        CancellationToken cancellationToken = default
    );
}
