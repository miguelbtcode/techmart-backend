using FluentValidation;

namespace TechMart.Auth.Application.Features.Authentication.Queries.ValidateRefreshToken;

internal sealed class ValidateRefreshTokenQueryValidator
    : AbstractValidator<ValidateRefreshTokenQuery>
{
    public ValidateRefreshTokenQueryValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty()
            .WithMessage("Refresh token must not be empty.")
            .MaximumLength(1000)
            .WithMessage("Refresh token must not exceed 1000 characters.");
    }
}
