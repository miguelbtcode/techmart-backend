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
