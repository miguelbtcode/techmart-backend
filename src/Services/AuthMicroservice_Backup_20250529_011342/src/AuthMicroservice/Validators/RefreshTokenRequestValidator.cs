using AuthMicroservice.Models.DTOs;
using FastEndpoints;
using FluentValidation;

namespace AuthMicroservice.Validators;

public class RefreshTokenRequestValidator : Validator<RefreshTokenRequest>
{
    public RefreshTokenRequestValidator()
    {
        RuleFor(x => x.RefreshToken).NotEmpty().WithMessage("El refresh token es requerido");
    }
}
