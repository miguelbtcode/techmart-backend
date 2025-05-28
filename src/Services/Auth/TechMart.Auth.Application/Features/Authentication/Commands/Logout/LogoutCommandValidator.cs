using FluentValidation;

namespace TechMart.Auth.Application.Features.Authentication.Commands.Logout;

internal sealed class LogoutCommandValidator : AbstractValidator<LogoutCommand>
{
    public LogoutCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty().WithMessage("User ID must not be empty.");
    }
}
