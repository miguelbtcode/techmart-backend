using FluentValidation;

namespace TechMart.Auth.Application.Features.Authentication.Commands.Login;

internal sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("El email es requerido")
            .WithErrorCode("Email.Required");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("La contraseña es requerida")
            .WithErrorCode("Password.Required");
    }
}
