namespace TechMart.Auth.Application.Contracts.Infrastructure;

/// <summary>
/// Service for sending emails
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Sends a welcome email to a new user
    /// </summary>
    Task SendWelcomeEmailAsync(
        string email,
        string firstName,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Sends an email confirmation email
    /// </summary>
    Task SendEmailConfirmationAsync(
        string email,
        string firstName,
        string confirmationToken,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Sends a password reset email
    /// </summary>
    Task SendPasswordResetEmailAsync(
        string email,
        string firstName,
        string resetToken,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Sends a password changed notification email
    /// </summary>
    Task SendPasswordChangedNotificationAsync(
        string email,
        string firstName,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Sends a generic notification email
    /// </summary>
    Task SendNotificationEmailAsync(
        string email,
        string subject,
        string htmlContent,
        string? plainTextContent = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Sends an email with template
    /// </summary>
    Task SendTemplatedEmailAsync(
        string email,
        string templateName,
        object templateData,
        CancellationToken cancellationToken = default
    );
}
