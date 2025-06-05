using System.Security.Cryptography;
using TechMart.Auth.Application.Common.Results;
using TechMart.Auth.Application.Features.PasswordReset.Commands.ResetPassword;
using TechMart.Auth.Domain.Entities;
using TechMart.Auth.Domain.Interfaces;
using MediatR;

namespace TechMart.Auth.Application.Features.PasswordReset.Commands.ForgotPassword;

public class ForgotPasswordHandler : IRequestHandler<ForgotPasswordCommand, Result>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordResetTokenRepository _tokenRepository;
    private readonly IEmailService _emailService;

    public ForgotPasswordHandler(
        IUserRepository userRepository,
        IPasswordResetTokenRepository tokenRepository,
        IEmailService emailService
    )
    {
        _userRepository = userRepository;
        _tokenRepository = tokenRepository;
        _emailService = emailService;
    }

    public async Task<Result> Handle(
        ForgotPasswordCommand request,
        CancellationToken cancellationToken
    )
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);

        if (user == null || !user.IsActive)
        {
            // Return success for security (dont reveal if email exists)
            return Result.Success("Reset instructions sent if email exists");
        }

        // Generate secure token
        var tokenBytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(tokenBytes);
        var token = Convert.ToBase64String(tokenBytes);

        // Create reset token
        var resetToken = new PasswordResetToken
        {
            Token = token,
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddHours(1), // 1 hour expiry
            CreatedAt = DateTime.UtcNow,
        };

        await _tokenRepository.CreateAsync(resetToken);

        // Send email
        await _emailService.SendPasswordResetEmailAsync(user.Email, token);

        return Result.Success("Reset instructions sent if email exists");
    }
}

public class ResetPasswordHandler : IRequestHandler<ResetPasswordCommand, Result>
{
    private readonly IPasswordResetTokenRepository _tokenRepository;
    private readonly IUserRepository _userRepository;
    private readonly IEmailService _emailService;

    public ResetPasswordHandler(
        IPasswordResetTokenRepository tokenRepository,
        IUserRepository userRepository,
        IEmailService emailService
    )
    {
        _tokenRepository = tokenRepository;
        _userRepository = userRepository;
        _emailService = emailService;
    }

    public async Task<Result> Handle(
        ResetPasswordCommand request,
        CancellationToken cancellationToken
    )
    {
        var resetToken = await _tokenRepository.GetByTokenAsync(request.Token);

        if (resetToken == null || !resetToken.IsValid)
        {
            return Result.Failure("Invalid or expired reset token");
        }

        var user = resetToken.User;

        // Update password
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        user.UpdatedAt = DateTime.UtcNow;

        await _userRepository.UpdateAsync(user);

        // Mark token as used
        resetToken.MarkAsUsed();
        await _tokenRepository.UpdateAsync(resetToken);

        // Send confirmation email
        await _emailService.SendPasswordChangedNotificationAsync(user.Email, user.Username);

        return Result.Success("Password reset successfully");
    }
}
