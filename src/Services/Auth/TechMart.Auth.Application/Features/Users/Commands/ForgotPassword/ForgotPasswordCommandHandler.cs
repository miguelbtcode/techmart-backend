using TechMart.Auth.Application.Abstractions.Contracts;
using TechMart.Auth.Application.Abstractions.Messaging;
using TechMart.Auth.Domain.Primitives;
using TechMart.Auth.Domain.Users.ValueObjects;

namespace TechMart.Auth.Application.Features.Users.Commands.ForgotPassword;

internal sealed class ForgotPasswordCommandHandler(
    IUnitOfWork unitOfWork,
    IEmailService emailService,
    IPasswordResetService passwordResetService
) : ICommandHandler<ForgotPasswordCommand>
{
    public async Task<Result> Handle(
        ForgotPasswordCommand request,
        CancellationToken cancellationToken
    )
    {
        var emailResult = Email.Create(request.Email);
        if (emailResult.IsFailure)
            return emailResult.Error;

        var user = await unitOfWork.Users.GetByEmailAsync(emailResult.Value, cancellationToken);

        // Don't reveal if user exists or not for security
        if (user is null)
            return Result.Success();

        // Generate reset token
        var resetToken = await passwordResetService.GenerateResetTokenAsync(
            user.Id,
            cancellationToken
        );

        // Send email
        await emailService.SendPasswordResetEmailAsync(
            user.Email.Value,
            user.FirstName,
            resetToken,
            cancellationToken
        );

        return Result.Success();
    }
}
