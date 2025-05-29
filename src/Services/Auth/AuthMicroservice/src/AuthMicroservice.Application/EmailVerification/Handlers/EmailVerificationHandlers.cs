using System.Security.Cryptography;
using AuthMicroservice.Application.Common.Results;
using AuthMicroservice.Application.EmailVerification.Commands;
using AuthMicroservice.Domain.Entities;
using AuthMicroservice.Domain.Events;
using AuthMicroservice.Domain.Interfaces;
using MediatR;

namespace AuthMicroservice.Application.EmailVerification.Handlers;

public class SendVerificationHandler : IRequestHandler<SendVerificationCommand, Result>
{
    private readonly IUserRepository _userRepository;
    private readonly IEmailVerificationTokenRepository _tokenRepository;
    private readonly IEmailService _emailService;

    public SendVerificationHandler(
        IUserRepository userRepository,
        IEmailVerificationTokenRepository tokenRepository,
        IEmailService emailService
    )
    {
        _userRepository = userRepository;
        _tokenRepository = tokenRepository;
        _emailService = emailService;
    }

    public async Task<Result> Handle(
        SendVerificationCommand request,
        CancellationToken cancellationToken
    )
    {
        var user = await _userRepository.GetByIdAsync(request.UserId);

        if (user == null)
        {
            return Result.Failure("User not found");
        }

        if (user.IsEmailVerified)
        {
            return Result.Failure("Email already verified");
        }

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
            ExpiresAt = DateTime.UtcNow.AddHours(24), // 24 hours expiry
            CreatedAt = DateTime.UtcNow,
        };

        await _tokenRepository.CreateAsync(verificationToken);

        // Send email
        await _emailService.SendVerificationEmailAsync(user.Email, token);

        return Result.Success("Verification email sent");
    }
}

public class VerifyEmailHandler : IRequestHandler<VerifyEmailCommand, Result>
{
    private readonly IEmailVerificationTokenRepository _tokenRepository;
    private readonly IUserRepository _userRepository;
    private readonly IEmailService _emailService;
    private readonly IMediator _mediator;

    public VerifyEmailHandler(
        IEmailVerificationTokenRepository tokenRepository,
        IUserRepository userRepository,
        IEmailService emailService,
        IMediator mediator
    )
    {
        _tokenRepository = tokenRepository;
        _userRepository = userRepository;
        _emailService = emailService;
        _mediator = mediator;
    }

    public async Task<Result> Handle(
        VerifyEmailCommand request,
        CancellationToken cancellationToken
    )
    {
        var verificationToken = await _tokenRepository.GetByTokenAsync(request.Token);

        if (verificationToken == null || !verificationToken.IsValid)
        {
            return Result.Failure("Invalid or expired verification token");
        }

        var user = verificationToken.User;

        // Verify email
        user.MarkEmailAsVerified();
        await _userRepository.UpdateAsync(user);

        // Mark token as used
        verificationToken.MarkAsUsed();
        await _tokenRepository.UpdateAsync(verificationToken);

        // Send welcome email
        await _emailService.SendWelcomeEmailAsync(user.Email, user.Username);

        // Publish event
        await _mediator.Publish(
            new EmailVerifiedEvent(user.Id, user.Email, DateTime.UtcNow),
            cancellationToken
        );

        return Result.Success("Email verified successfully");
    }
}
