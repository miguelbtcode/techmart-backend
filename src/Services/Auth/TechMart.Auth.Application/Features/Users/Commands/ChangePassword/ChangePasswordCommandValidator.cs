using TechMart.Auth.Application.Common.Validators;

namespace TechMart.Auth.Application.Features.Users.Commands.ChangePassword;

internal sealed class ChangePasswordCommandValidator : BaseValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        ValidateUserId(x => x.UserId);
        ValidatePassword(x => x.NewPassword);
        ValidateRequiredString(x => x.CurrentPassword, "Current Password");
    }
}
