using FluentValidation;

namespace TechMart.Auth.Application.Features.Users.Commands.CreateUser;

internal sealed class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("El email es requerido")
            .WithErrorCode("Email.Required");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("La contraseña es requerida")
            .WithErrorCode("Password.Required");

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("El nombre es requerido")
            .WithErrorCode("FirstName.Required");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("El apellido es requerido")
            .WithErrorCode("LastName.Required");
    }
}
