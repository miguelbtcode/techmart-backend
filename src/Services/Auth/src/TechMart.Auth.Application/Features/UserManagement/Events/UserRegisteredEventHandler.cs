using TechMart.Auth.Application.Features.EmailVerification.Commands.SendVerificationEmail;
using TechMart.Auth.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace TechMart.Auth.Application.Features.UserManagement.Events;

public class UserRegisteredEventHandler : INotificationHandler<UserRegisteredEvent>
{
    private readonly IMediator _mediator;
    private readonly ILogger<UserRegisteredEventHandler> _logger;

    public UserRegisteredEventHandler(
        IMediator mediator,
        ILogger<UserRegisteredEventHandler> logger
    )
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task Handle(UserRegisteredEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing user registration for {UserId}", notification.UserId);

        try
        {
            var command = new SendVerificationEmailCommand(notification.UserId);
            await _mediator.Send(command, cancellationToken);

            _logger.LogInformation(
                "Verification email queued for user {UserId}",
                notification.UserId
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to send verification email for user {UserId}",
                notification.UserId
            );
            // Don't throw - registration should still succeed even if email fails
        }
    }
}
