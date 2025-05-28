using FluentValidation;

namespace TechMart.Auth.Application.Features.Users.Commands.ResetPassword;

internal sealed class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("El correo electrónico es requerido")
            .WithErrorCode("Email.Required");

        RuleFor(x => x.Token)
            .NotEmpty()
            .WithMessage("El token es requerido")
            .WithErrorCode("Token.Required");

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .WithMessage("La nueva contraseña es requerida")
            .WithErrorCode("NewPassword.Required");
    }
}
