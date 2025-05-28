using TechMart.Auth.Application.Contracts.Authentication;
using TechMart.Auth.Application.Messaging.Commands;
using TechMart.Auth.Domain.Primitives;
using TechMart.Auth.Domain.Users.Errors;
using TechMart.Auth.Domain.Users.ValueObjects;

namespace TechMart.Auth.Application.Features.Users.Commands.ChangePassword;

internal sealed class ChangePasswordCommandHandler(
    IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher
) : ICommandHandler<ChangePasswordCommand>
{
    public async Task<Result> Handle(
        ChangePasswordCommand request,
        CancellationToken cancellationToken
    )
    {
        var userId = UserId.From(request.UserId);
        var user = await unitOfWork.Users.GetByIdAsync(userId, cancellationToken);

        if (user is null)
            return UserErrors.NotFound(userId);

        // Verify current password
        if (!passwordHasher.VerifyPassword(request.CurrentPassword, user.PasswordHash))
            return UserErrors.InvalidCredentials();

        // Validate new password
        var newPasswordResult = Password.Create(request.NewPassword);
        if (newPasswordResult.IsFailure)
            return newPasswordResult.Error;

        // Hash new password
        var newPasswordHash = passwordHasher.HashPassword(newPasswordResult.Value.Value);

        // Change password
        var changeResult = user.ChangePassword(newPasswordHash, false, userId);
        if (changeResult.IsFailure)
            return changeResult.Error;

        await unitOfWork.SaveChangesAsync(cancellationToken);
        await unitOfWork.DispatchDomainEventsAsync(cancellationToken);

        return Result.Success();
    }
}
