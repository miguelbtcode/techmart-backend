using FluentValidation;

namespace TechMart.Auth.Application.Features.Authentication.Commands.RefreshToken;

internal sealed class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty()
            .WithMessage("Refresh token is required")
            .WithErrorCode("RefreshToken.Required");
    }
}
