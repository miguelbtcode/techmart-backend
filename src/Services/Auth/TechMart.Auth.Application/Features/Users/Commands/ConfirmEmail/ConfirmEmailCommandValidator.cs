using FluentValidation;

namespace TechMart.Auth.Application.Features.Users.Commands.ConfirmEmail;

internal sealed class ConfirmEmailCommandValidator : AbstractValidator<ConfirmEmailCommand>
{
    public ConfirmEmailCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("El email es requerido")
            .WithErrorCode("Email.Required");

        RuleFor(x => x.Token)
            .NotEmpty()
            .WithMessage("El token es requerido")
            .WithErrorCode("Token.Required");
    }
}
