using System.Net;
using System.Net.Mail;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TechMart.Auth.Application.Abstractions.Contracts;
using TechMart.Auth.Infrastructure.Services.Settings;

namespace TechMart.Auth.Infrastructure.Services;

/// <summary>
/// Email service implementation using SMTP
/// Supports HTML templates for different email types
/// </summary>
public sealed class EmailService : IEmailService
{
    private readonly EmailSettings _emailSettings;
    private readonly ILogger<EmailService> _logger;
    private readonly SmtpClient _smtpClient;

    public EmailService(IOptions<EmailSettings> emailSettings, ILogger<EmailService> logger)
    {
        _emailSettings = emailSettings.Value;
        _logger = logger;

        // Configure SMTP client
        _smtpClient = new SmtpClient
        {
            Host = "smtp.gmail.com", // Configure based on your email provider
            Port = 587,
            EnableSsl = true,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(_emailSettings.FromEmail, _emailSettings.ApiKey),
        };
    }

    public async Task SendWelcomeEmailAsync(
        string email,
        string firstName,
        CancellationToken cancellationToken = default
    )
    {
        if (!_emailSettings.EnableEmailSending)
        {
            _logger.LogInformation(
                "Email sending is disabled. Welcome email for {Email} skipped",
                email
            );
            return;
        }

        try
        {
            var subject = "¡Bienvenido a TechMart!";
            var body = BuildWelcomeEmailTemplate(firstName);

            await SendEmailAsync(email, subject, body, cancellationToken);

            _logger.LogInformation("Welcome email sent successfully to {Email}", email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send welcome email to {Email}", email);
            throw;
        }
    }

    public async Task SendEmailConfirmationAsync(
        string email,
        string firstName,
        string confirmationToken,
        CancellationToken cancellationToken = default
    )
    {
        if (!_emailSettings.EnableEmailSending)
        {
            _logger.LogInformation(
                "Email sending is disabled. Confirmation email for {Email} skipped",
                email
            );
            return;
        }

        try
        {
            var subject = "Confirma tu cuenta en TechMart";
            var confirmationUrl =
                $"{_emailSettings.BaseUrl}/auth/confirm-email?email={Uri.EscapeDataString(email)}&token={Uri.EscapeDataString(confirmationToken)}";
            var body = BuildEmailConfirmationTemplate(firstName, confirmationUrl);

            await SendEmailAsync(email, subject, body, cancellationToken);

            _logger.LogInformation("Email confirmation sent successfully to {Email}", email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email confirmation to {Email}", email);
            throw;
        }
    }

    public async Task SendPasswordResetEmailAsync(
        string email,
        string firstName,
        string resetToken,
        CancellationToken cancellationToken = default
    )
    {
        if (!_emailSettings.EnableEmailSending)
        {
            _logger.LogInformation(
                "Email sending is disabled. Password reset email for {Email} skipped",
                email
            );
            return;
        }

        try
        {
            var subject = "Restablece tu contraseña - TechMart";
            var resetUrl =
                $"{_emailSettings.BaseUrl}/auth/reset-password?email={Uri.EscapeDataString(email)}&token={Uri.EscapeDataString(resetToken)}";
            var body = BuildPasswordResetTemplate(firstName, resetUrl);

            await SendEmailAsync(email, subject, body, cancellationToken);

            _logger.LogInformation("Password reset email sent successfully to {Email}", email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send password reset email to {Email}", email);
            throw;
        }
    }

    private async Task SendEmailAsync(
        string to,
        string subject,
        string body,
        CancellationToken cancellationToken
    )
    {
        var mailMessage = new MailMessage
        {
            From = new MailAddress(_emailSettings.FromEmail, _emailSettings.FromName),
            Subject = subject,
            Body = body,
            IsBodyHtml = true,
            BodyEncoding = Encoding.UTF8,
            SubjectEncoding = Encoding.UTF8,
        };

        mailMessage.To.Add(to);

        await _smtpClient.SendMailAsync(mailMessage, cancellationToken);
    }

    private string BuildWelcomeEmailTemplate(string firstName)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset=""utf-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Bienvenido a TechMart</title>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 30px; text-align: center; border-radius: 10px 10px 0 0; }}
        .content {{ background: #f8f9fa; padding: 30px; border-radius: 0 0 10px 10px; }}
        .button {{ display: inline-block; background: #007bff; color: white; padding: 12px 30px; text-decoration: none; border-radius: 5px; font-weight: bold; }}
        .footer {{ margin-top: 30px; padding-top: 20px; border-top: 1px solid #ddd; color: #666; font-size: 14px; }}
    </style>
</head>
<body>
    <div class=""header"">
        <h1>¡Bienvenido a TechMart!</h1>
    </div>
    <div class=""content"">
        <h2>Hola {firstName},</h2>
        <p>¡Gracias por registrarte en TechMart! Estamos emocionados de tenerte como parte de nuestra comunidad.</p>
        <p>En TechMart encontrarás los mejores productos tecnológicos con la mejor calidad y precios competitivos.</p>
        <p>Para comenzar a explorar nuestro catálogo, haz clic en el siguiente botón:</p>
        <p style=""text-align: center; margin: 30px 0;"">
            <a href=""{_emailSettings.BaseUrl}"" class=""button"">Explorar Productos</a>
        </p>
        <p>Si tienes alguna pregunta, no dudes en contactarnos. ¡Estamos aquí para ayudarte!</p>
        <p>Saludos cordiales,<br>El equipo de TechMart</p>
    </div>
    <div class=""footer"">
        <p>Este es un email automático, por favor no respondas a este mensaje.</p>
        <p>© 2024 TechMart. Todos los derechos reservados.</p>
    </div>
</body>
</html>";
    }

    private string BuildEmailConfirmationTemplate(string firstName, string confirmationUrl)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset=""utf-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Confirma tu cuenta</title>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: linear-gradient(135deg, #28a745 0%, #20c997 100%); color: white; padding: 30px; text-align: center; border-radius: 10px 10px 0 0; }}
        .content {{ background: #f8f9fa; padding: 30px; border-radius: 0 0 10px 10px; }}
        .button {{ display: inline-block; background: #28a745; color: white; padding: 12px 30px; text-decoration: none; border-radius: 5px; font-weight: bold; }}
        .footer {{ margin-top: 30px; padding-top: 20px; border-top: 1px solid #ddd; color: #666; font-size: 14px; }}
        .warning {{ background: #fff3cd; border: 1px solid #ffeeba; color: #856404; padding: 15px; border-radius: 5px; margin: 20px 0; }}
    </style>
</head>
<body>
    <div class=""header"">
        <h1>Confirma tu cuenta</h1>
    </div>
    <div class=""content"">
        <h2>Hola {firstName},</h2>
        <p>Para completar tu registro en TechMart, necesitamos verificar tu dirección de email.</p>
        <p>Haz clic en el siguiente botón para confirmar tu cuenta:</p>
        <p style=""text-align: center; margin: 30px 0;"">
            <a href=""{confirmationUrl}"" class=""button"">Confirmar Cuenta</a>
        </p>
        <div class=""warning"">
            <strong>Importante:</strong> Este enlace expirará en 24 horas por razones de seguridad.
        </div>
        <p>Si no puedes hacer clic en el botón, copia y pega el siguiente enlace en tu navegador:</p>
        <p style=""word-break: break-all; background: #e9ecef; padding: 10px; border-radius: 5px; font-family: monospace;"">
            {confirmationUrl}
        </p>
        <p>Si no te registraste en TechMart, puedes ignorar este email de forma segura.</p>
        <p>Saludos cordiales,<br>El equipo de TechMart</p>
    </div>
    <div class=""footer"">
        <p>Este es un email automático, por favor no respondas a este mensaje.</p>
        <p>© 2024 TechMart. Todos los derechos reservados.</p>
    </div>
</body>
</html>";
    }

    private string BuildPasswordResetTemplate(string firstName, string resetUrl)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset=""utf-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Restablece tu contraseña</title>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: linear-gradient(135deg, #dc3545 0%, #fd7e14 100%); color: white; padding: 30px; text-align: center; border-radius: 10px 10px 0 0; }}
        .content {{ background: #f8f9fa; padding: 30px; border-radius: 0 0 10px 10px; }}
        .button {{ display: inline-block; background: #dc3545; color: white; padding: 12px 30px; text-decoration: none; border-radius: 5px; font-weight: bold; }}
        .footer {{ margin-top: 30px; padding-top: 20px; border-top: 1px solid #ddd; color: #666; font-size: 14px; }}
        .warning {{ background: #f8d7da; border: 1px solid #f5c6cb; color: #721c24; padding: 15px; border-radius: 5px; margin: 20px 0; }}
    </style>
</head>
<body>
    <div class=""header"">
        <h1>Restablece tu contraseña</h1>
    </div>
    <div class=""content"">
        <h2>Hola {firstName},</h2>
        <p>Recibimos una solicitud para restablecer la contraseña de tu cuenta en TechMart.</p>
        <p>Para crear una nueva contraseña, haz clic en el siguiente botón:</p>
        <p style=""text-align: center; margin: 30px 0;"">
            <a href=""{resetUrl}"" class=""button"">Restablecer Contraseña</a>
        </p>
        <div class=""warning"">
            <strong>Atención:</strong> Este enlace expirará en 1 hora por razones de seguridad.
        </div>
        <p>Si no puedes hacer clic en el botón, copia y pega el siguiente enlace en tu navegador:</p>
        <p style=""word-break: break-all; background: #e9ecef; padding: 10px; border-radius: 5px; font-family: monospace;"">
            {resetUrl}
        </p>
        <p><strong>¿No solicitaste este cambio?</strong> Si no solicitaste restablecer tu contraseña, puedes ignorar este email de forma segura. Tu contraseña no será modificada.</p>
        <p>Saludos cordiales,<br>El equipo de TechMart</p>
    </div>
    <div class=""footer"">
        <p>Este es un email automático, por favor no respondas a este mensaje.</p>
        <p>© 2024 TechMart. Todos los derechos reservados.</p>
    </div>
</body>
</html>";
    }

    public void Dispose()
    {
        _smtpClient?.Dispose();
    }
}
