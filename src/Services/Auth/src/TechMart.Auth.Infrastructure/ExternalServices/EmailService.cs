using System.Net;
using System.Net.Mail;
using AuthMicroservice.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace AuthMicroservice.Infrastructure.ExternalServices;

public class EmailService : IEmailService
{
    private readonly EmailSettings _emailSettings;
    private readonly ILogger<EmailService> _logger;

    public EmailService(EmailSettings emailSettings, ILogger<EmailService> logger)
    {
        _emailSettings = emailSettings;
        _logger = logger;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        try
        {
            using var client = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort)
            {
                Credentials = new NetworkCredential(
                    _emailSettings.Username,
                    _emailSettings.Password
                ),
                EnableSsl = _emailSettings.EnableSsl,
            };

            var message = new MailMessage
            {
                From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };

            message.To.Add(to);

            await client.SendMailAsync(message);
            _logger.LogInformation("Email sent successfully to {Email}", to);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {Email}", to);
            throw;
        }
    }

    public async Task SendVerificationEmailAsync(string email, string token)
    {
        var subject = "Verify your email address";
        var body =
            $@"
            <h2>Email Verification</h2>
            <p>Please click the link below to verify your email address:</p>
            <a href=""https://localhost:7001/api/auth/verify-email?token={token}"">Verify Email</a>
            <p>If the link doesn't work, copy and paste this URL into your browser:</p>
            <p>https://localhost:7001/api/auth/verify-email?token={token}</p>
        ";

        await SendEmailAsync(email, subject, body);
    }

    public async Task SendPasswordResetEmailAsync(string email, string token)
    {
        var subject = "Reset your password";
        var body =
            $@"
            <h2>Password Reset</h2>
            <p>Please click the link below to reset your password:</p>
            <a href=""https://localhost:7001/api/auth/reset-password?token={token}"">Reset Password</a>
            <p>If the link doesn't work, copy and paste this URL into your browser:</p>
            <p>https://localhost:7001/api/auth/reset-password?token={token}</p>
            <p>This link will expire in 24 hours.</p>
        ";

        await SendEmailAsync(email, subject, body);
    }

    public async Task SendWelcomeEmailAsync(string email, string firstName)
    {
        var subject = "Welcome to AuthMicroservice!";
        var body =
            $@"
        <div style=""font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;"">
            <h2 style=""color: #333; text-align: center;"">Welcome to AuthMicroservice! 🎉</h2>
            
            <div style=""background-color: #f8f9fa; padding: 20px; border-radius: 8px; margin: 20px 0;"">
                <h3 style=""color: #28a745;"">Hello {firstName}!</h3>
                <p style=""font-size: 16px; line-height: 1.6;"">
                    Welcome to our platform! We're excited to have you on board.
                </p>
                
                <div style=""background-color: white; padding: 15px; border-radius: 5px; margin: 15px 0;"">
                    <h4 style=""color: #495057;"">What you can do now:</h4>
                    <ul style=""color: #6c757d;"">
                        <li>Complete your profile</li>
                        <li>Explore our features</li>
                        <li>Connect with social accounts</li>
                        <li>Set up two-factor authentication for extra security</li>
                    </ul>
                </div>
                
                <p style=""font-size: 14px; color: #6c757d; margin-top: 20px;"">
                    If you have any questions, feel free to contact our support team.
                </p>
            </div>
            
            <div style=""text-align: center; margin-top: 30px; padding: 20px; background-color: #e9ecef; border-radius: 8px;"">
                <p style=""margin: 0; color: #6c757d; font-size: 12px;"">
                    Thanks for joining us!<br>
                    The AuthMicroservice Team
                </p>
            </div>
        </div>
    ";

        await SendEmailAsync(email, subject, body);
    }

    public async Task SendPasswordChangedNotificationAsync(string email, string firstName)
    {
        var subject = "Password Changed Successfully";
        var body =
            $@"
        <div style=""font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;"">
            <h2 style=""color: #333; text-align: center;"">Password Changed Successfully 🔐</h2>
            
            <div style=""background-color: #f8f9fa; padding: 20px; border-radius: 8px; margin: 20px 0;"">
                <h3 style=""color: #28a745;"">Hello {firstName}!</h3>
                <p style=""font-size: 16px; line-height: 1.6;"">
                    Your password has been successfully changed on <strong>{DateTime.UtcNow:MMM dd, yyyy 'at' HH:mm} UTC</strong>.
                </p>
                
                <div style=""background-color: #d4edda; border: 1px solid #c3e6cb; padding: 15px; border-radius: 5px; margin: 15px 0;"">
                    <h4 style=""color: #155724; margin: 0 0 10px 0;"">✅ Security Update Confirmed</h4>
                    <p style=""color: #155724; margin: 0; font-size: 14px;"">
                        Your account is now secured with your new password.
                    </p>
                </div>
                
                <div style=""background-color: #fff3cd; border: 1px solid #ffeaa7; padding: 15px; border-radius: 5px; margin: 15px 0;"">
                    <h4 style=""color: #856404; margin: 0 0 10px 0;"">⚠️ Didn't make this change?</h4>
                    <p style=""color: #856404; margin: 0; font-size: 14px;"">
                        If you didn't change your password, please contact our support team immediately 
                        and consider securing your account.
                    </p>
                </div>
                
                <div style=""background-color: white; padding: 15px; border-radius: 5px; margin: 15px 0;"">
                    <h4 style=""color: #495057;"">Security Tips:</h4>
                    <ul style=""color: #6c757d; font-size: 14px;"">
                        <li>Use a strong, unique password</li>
                        <li>Enable two-factor authentication</li>
                        <li>Don't share your password with anyone</li>
                        <li>Log out from shared devices</li>
                    </ul>
                </div>
            </div>
            
            <div style=""text-align: center; margin-top: 30px; padding: 20px; background-color: #e9ecef; border-radius: 8px;"">
                <p style=""margin: 0; color: #6c757d; font-size: 12px;"">
                    This is an automated security notification.<br>
                    The AuthMicroservice Team
                </p>
            </div>
        </div>
    ";

        await SendEmailAsync(email, subject, body);
    }
}

public class EmailSettings
{
    public string SmtpServer { get; set; } = string.Empty;
    public int SmtpPort { get; set; } = 587;
    public bool EnableSsl { get; set; } = true;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string SenderEmail { get; set; } = string.Empty;
    public string SenderName { get; set; } = string.Empty;
}
