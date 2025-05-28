using TechMart.Auth.Application.Common.Extensions;
using TechMart.Auth.Application.Common.Validators;

namespace TechMart.Auth.Application.Features.Users.Commands.ForgotPassword;

internal sealed class ForgotPasswordCommandValidator : BaseValidator<ForgotPasswordCommand>
{
    public ForgotPasswordCommandValidator()
    {
        RuleFor(x => x.Email).ValidEmail();
    }
}
