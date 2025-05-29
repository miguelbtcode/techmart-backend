using FluentValidation;

namespace AuthMicroservice.Application.Features.Authentication.Commands.Login;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.EmailOrUsername)
            .NotEmpty()
            .WithMessage("Email or Username is required")
            .MaximumLength(100)
            .WithMessage("Email or Username must not exceed 100 characters");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required")
            .MinimumLength(6)
            .WithMessage("Password must be at least 6 characters long")
            .MaximumLength(100)
            .WithMessage("Password must not exceed 100 characters");
    }
}
