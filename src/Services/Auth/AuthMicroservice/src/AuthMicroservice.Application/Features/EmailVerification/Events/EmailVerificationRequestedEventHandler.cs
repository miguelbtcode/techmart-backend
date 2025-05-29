using AuthMicroservice.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AuthMicroservice.Application.Features.EmailVerification.Events;

public class EmailVerificationRequestedEventHandler
    : INotificationHandler<EmailVerificationRequestedEvent>
{
    private readonly IEmailService _emailService;
    private readonly ILogger<EmailVerificationRequestedEventHandler> _logger;

    public EmailVerificationRequestedEventHandler(
        IEmailService emailService,
        ILogger<EmailVerificationRequestedEventHandler> logger
    )
    {
        _emailService = emailService;
        _logger = logger;
    }

    public async Task Handle(
        EmailVerificationRequestedEvent notification,
        CancellationToken cancellationToken
    )
    {
        _logger.LogInformation(
            "Sending verification email to {Email} for user {UserId}",
            notification.Email,
            notification.UserId
        );

        try
        {
            await _emailService.SendVerificationEmailAsync(notification.Email, notification.Token);
            _logger.LogInformation(
                "Verification email sent successfully to {Email}",
                notification.Email
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to send verification email to {Email}",
                notification.Email
            );
            throw;
        }
    }
}
