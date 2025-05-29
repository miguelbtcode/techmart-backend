using AuthMicroservice.Models.DTOs;
using FastEndpoints;
using FluentValidation;

namespace AuthMicroservice.Validators;

public class RegisterRequestValidator : Validator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("El email es requerido")
            .EmailAddress()
            .WithMessage("Formato de email inválido")
            .MaximumLength(100)
            .WithMessage("El email no puede exceder 100 caracteres");

        RuleFor(x => x.Username)
            .NotEmpty()
            .WithMessage("El nombre de usuario es requerido")
            .MinimumLength(3)
            .WithMessage("El nombre de usuario debe tener al menos 3 caracteres")
            .MaximumLength(50)
            .WithMessage("El nombre de usuario no puede exceder 50 caracteres")
            .Matches("^[a-zA-Z0-9_.-]+$")
            .WithMessage(
                "El nombre de usuario solo puede contener letras, números, guiones y puntos"
            );

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("La contraseña es requerida")
            .MinimumLength(8)
            .WithMessage("La contraseña debe tener al menos 8 caracteres")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)")
            .WithMessage(
                "La contraseña debe contener al menos una minúscula, una mayúscula y un número"
            );

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty()
            .WithMessage("La confirmación de contraseña es requerida")
            .Equal(x => x.Password)
            .WithMessage("Las contraseñas no coinciden");

        RuleFor(x => x.FirstName)
            .MaximumLength(100)
            .WithMessage("El nombre no puede exceder 100 caracteres")
            .When(x => !string.IsNullOrEmpty(x.FirstName));

        RuleFor(x => x.LastName)
            .MaximumLength(100)
            .WithMessage("El apellido no puede exceder 100 caracteres")
            .When(x => !string.IsNullOrEmpty(x.LastName));
    }
}
