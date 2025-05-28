using Microsoft.Extensions.Logging;
using TechMart.Auth.Application.Common.Events;
using TechMart.Auth.Application.Contracts.Infrastructure;
using TechMart.Auth.Domain.Users.Events;

namespace TechMart.Auth.Application.Features.Users.Commands.CreateUser.EventHandlers;

/// <summary>
/// Handles sending email confirmation when a user is registered
/// </summary>
public sealed class SendEmailConfirmationHandler : IDomainEventHandler<UserCreatedEvent>
{
    private readonly IEmailService _emailService;
    private readonly IEmailConfirmationService _emailConfirmationService;
    private readonly ILogger<SendEmailConfirmationHandler> _logger;

    public SendEmailConfirmationHandler(
        IEmailService emailService,
        IEmailConfirmationService emailConfirmationService,
        ILogger<SendEmailConfirmationHandler> logger
    )
    {
        _emailService = emailService;
        _emailConfirmationService = emailConfirmationService;
        _logger = logger;
    }

    public async Task Handle(UserCreatedEvent domainEvent, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation(
                "Sending email confirmation to user {UserId} with email {Email}",
                domainEvent.UserId,
                domainEvent.Email
            );

            // 1. Generate email confirmation token
            var confirmationToken = await _emailConfirmationService.GenerateTokenAsync(
                domainEvent.Email,
                cancellationToken
            );

            // 2. Send confirmation email
            await _emailService.SendEmailConfirmationAsync(
                domainEvent.Email,
                domainEvent.FirstName,
                confirmationToken,
                cancellationToken
            );

            _logger.LogInformation(
                "Email confirmation sent successfully to user {UserId}",
                domainEvent.UserId
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to send email confirmation to user {UserId} with email {Email}",
                domainEvent.UserId,
                domainEvent.Email
            );

            // Decide si quieres re-throw o solo loggear
            // En este caso, no re-throw para que no falle el registro
            // El usuario puede pedir reenvío del email después
            throw;
        }
    }
}
