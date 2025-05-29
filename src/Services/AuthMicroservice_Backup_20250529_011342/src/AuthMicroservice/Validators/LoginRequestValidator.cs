using AuthMicroservice.Models.DTOs;
using FastEndpoints;
using FluentValidation;

namespace AuthMicroservice.Validators;

public class LoginRequestValidator : Validator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.EmailOrUsername)
            .NotEmpty()
            .WithMessage("Email o nombre de usuario es requerido")
            .MinimumLength(3)
            .WithMessage("Debe tener al menos 3 caracteres");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("La contraseña es requerida")
            .MinimumLength(1)
            .WithMessage("La contraseña no puede estar vacía");
    }
}
