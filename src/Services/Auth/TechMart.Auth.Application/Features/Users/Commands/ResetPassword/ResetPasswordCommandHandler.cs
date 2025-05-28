using TechMart.Auth.Application.Contracts.Authentication;
using TechMart.Auth.Application.Contracts.Infrastructure;
using TechMart.Auth.Application.Messaging.Commands;
using TechMart.Auth.Domain.Primitives;
using TechMart.Auth.Domain.Users.Errors;
using TechMart.Auth.Domain.Users.ValueObjects;

namespace TechMart.Auth.Application.Features.Users.Commands.ResetPassword;

internal sealed class ResetPasswordCommandHandler(
    IUnitOfWork unitOfWork,
    IPasswordResetService passwordResetService,
    IPasswordHasher passwordHasher
) : ICommandHandler<ResetPasswordCommand>
{
    public async Task<Result> Handle(
        ResetPasswordCommand request,
        CancellationToken cancellationToken
    )
    {
        // Validate email
        var emailResult = Email.Create(request.Email);
        if (emailResult.IsFailure)
            return emailResult.Error;

        // Validate token
        var userId = await passwordResetService.ValidateResetTokenAsync(
            request.Email,
            request.Token,
            cancellationToken
        );

        if (userId is null)
            return UserErrors.InvalidTokenResetPassword();

        // Get user
        var user = await unitOfWork.Users.GetByIdAsync(userId, cancellationToken);

        if (user is null)
            return UserErrors.NotFound(userId.Value);

        // Validate new password
        var passwordResult = Password.Create(request.NewPassword);
        if (passwordResult.IsFailure)
            return passwordResult.Error;

        // Hash new password
        var password = passwordResult.Value;
        var passwordHash = passwordHasher.HashPassword(password.Value);

        // Change password
        var changeResult = user.ChangePassword(passwordHash, true);
        if (changeResult.IsFailure)
            return changeResult.Error;

        // Invalidate reset token
        await passwordResetService.InvalidateTokenAsync(request.Token, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        await unitOfWork.DispatchDomainEventsAsync(cancellationToken);

        return Result.Success();
    }
}
