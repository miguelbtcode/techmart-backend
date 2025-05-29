#!/bin/bash
# generate-event-handlers.sh - Genera event handlers para emails

set -e

PROJECT_PATH="${1:-src/AuthMicroservice.Application}"
NAMESPACE="AuthMicroservice.Application"

echo "📧 Generando Event Handlers..."

# Verificar que estamos en el directorio correcto
if [ ! -f "AuthMicroservice.sln" ]; then
    echo "❌ No se encontró AuthMicroservice.sln. Ejecuta desde la raíz del proyecto."
    exit 1
fi

# Crear UserRegisteredEventHandler
echo "📝 Creando UserRegisteredEventHandler..."

USER_REGISTERED_HANDLER="$PROJECT_PATH/Features/UserManagement/Events/UserRegisteredEventHandler.cs"
mkdir -p "$(dirname "$USER_REGISTERED_HANDLER")"

cat > "$USER_REGISTERED_HANDLER" << 'EOF'
using AuthMicroservice.Application.Features.EmailVerification.Commands.SendVerificationEmail;
using AuthMicroservice.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AuthMicroservice.Application.Features.UserManagement.Events;

public class UserRegisteredEventHandler : INotificationHandler<UserRegisteredEvent>
{
    private readonly IMediator _mediator;
    private readonly ILogger<UserRegisteredEventHandler> _logger;

    public UserRegisteredEventHandler(IMediator mediator, ILogger<UserRegisteredEventHandler> logger)
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
            
            _logger.LogInformation("Verification email queued for user {UserId}", notification.UserId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send verification email for user {UserId}", notification.UserId);
            // Don't throw - registration should still succeed even if email fails
        }
    }
}
EOF

echo "✅ Created: UserRegisteredEventHandler"

# Crear SendVerificationEmailCommand
echo "📝 Creando SendVerificationEmailCommand..."

SEND_VERIFICATION_CMD="$PROJECT_PATH/Features/EmailVerification/Commands/SendVerificationEmail/SendVerificationEmailCommand.cs"
mkdir -p "$(dirname "$SEND_VERIFICATION_CMD")"

cat > "$SEND_VERIFICATION_CMD" << 'EOF'
using AuthMicroservice.Application.Common.Results;
using MediatR;

namespace AuthMicroservice.Application.Features.EmailVerification.Commands.SendVerificationEmail;

public record SendVerificationEmailCommand(int UserId) : IRequest<Result>;
EOF

echo "✅ Created: SendVerificationEmailCommand"

# Crear SendVerificationEmailCommandHandler
echo "📝 Creando SendVerificationEmailCommandHandler..."

SEND_VERIFICATION_HANDLER="$PROJECT_PATH/Features/EmailVerification/Commands/SendVerificationEmail/SendVerificationEmailCommandHandler.cs"

cat > "$SEND_VERIFICATION_HANDLER" << 'EOF'
using System.Security.Cryptography;
using AuthMicroservice.Application.Common.Results;
using AuthMicroservice.Application.Features.EmailVerification.Events;
using AuthMicroservice.Domain.Entities;
using AuthMicroservice.Domain.Interfaces;
using MediatR;

namespace AuthMicroservice.Application.Features.EmailVerification.Commands.SendVerificationEmail;

public class SendVerificationEmailCommandHandler : IRequestHandler<SendVerificationEmailCommand, Result>
{
    private readonly IUserRepository _userRepository;
    private readonly IEmailVerificationTokenRepository _tokenRepository;
    private readonly IMediator _mediator;

    public SendVerificationEmailCommandHandler(
        IUserRepository userRepository,
        IEmailVerificationTokenRepository tokenRepository,
        IMediator mediator)
    {
        _userRepository = userRepository;
        _tokenRepository = tokenRepository;
        _mediator = mediator;
    }

    public async Task<Result> Handle(SendVerificationEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId);
        if (user == null)
            return Result.Failure("User not found");

        if (user.IsEmailVerified)
            return Result.Failure("Email already verified");

        // Generate secure token
        var tokenBytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(tokenBytes);
        var token = Convert.ToBase64String(tokenBytes);

        // Create verification token
        var verificationToken = new EmailVerificationToken
        {
            Token = token,
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddHours(24),
            CreatedAt = DateTime.UtcNow,
        };

        await _tokenRepository.CreateAsync(verificationToken);

        // Publish event to handle email sending
        await _mediator.Publish(new EmailVerificationRequestedEvent(
            user.Id, 
            user.Email, 
            token, 
            DateTime.UtcNow), cancellationToken);

        return Result.Success("Verification email will be sent");
    }
}
EOF

echo "✅ Created: SendVerificationEmailCommandHandler"

# Crear EmailVerificationRequestedEvent
echo "📝 Creando EmailVerificationRequestedEvent..."

EMAIL_VERIFICATION_EVENT="$PROJECT_PATH/Features/EmailVerification/Events/EmailVerificationRequestedEvent.cs"
mkdir -p "$(dirname "$EMAIL_VERIFICATION_EVENT")"

cat > "$EMAIL_VERIFICATION_EVENT" << 'EOF'
using MediatR;

namespace AuthMicroservice.Application.Features.EmailVerification.Events;

public record EmailVerificationRequestedEvent(
    int UserId,
    string Email,
    string Token,
    DateTime OccurredAt
) : INotification;
EOF

echo "✅ Created: EmailVerificationRequestedEvent"

# Crear EmailVerificationRequestedEventHandler
echo "📝 Creando EmailVerificationRequestedEventHandler..."

EMAIL_VERIFICATION_HANDLER="$PROJECT_PATH/Features/EmailVerification/Events/EmailVerificationRequestedEventHandler.cs"

cat > "$EMAIL_VERIFICATION_HANDLER" << 'EOF'
using AuthMicroservice.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AuthMicroservice.Application.Features.EmailVerification.Events;

public class EmailVerificationRequestedEventHandler : INotificationHandler<EmailVerificationRequestedEvent>
{
    private readonly IEmailService _emailService;
    private readonly ILogger<EmailVerificationRequestedEventHandler> _logger;

    public EmailVerificationRequestedEventHandler(
        IEmailService emailService, 
        ILogger<EmailVerificationRequestedEventHandler> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }

    public async Task Handle(EmailVerificationRequestedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Sending verification email to {Email} for user {UserId}", 
            notification.Email, notification.UserId);

        try
        {
            await _emailService.SendVerificationEmailAsync(notification.Email, notification.Token);
            _logger.LogInformation("Verification email sent successfully to {Email}", notification.Email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send verification email to {Email}", notification.Email);
            throw;
        }
    }
}
EOF

echo "✅ Created: EmailVerificationRequestedEventHandler"

echo ""
echo "✅ Event Handlers generados exitosamente!"
echo ""
echo "📋 Próximos pasos:"
echo "  1. Compilar para verificar: dotnet build"
echo "  2. Actualizar el RegisterHandler para usar el nuevo evento"
echo "  3. Probar el flujo completo de registro"