namespace AuthMicroservice.Domain.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body);

    Task SendVerificationEmailAsync(string email, string token);

    Task SendPasswordResetEmailAsync(string email, string token);

    Task SendWelcomeEmailAsync(string email, string firstName);
    Task SendPasswordChangedNotificationAsync(string email, string firstName);
}
