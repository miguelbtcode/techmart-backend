using FluentValidation;

namespace TechMart.Auth.Application.Features.Roles.Commands.RemoveRole;

internal sealed class RemoveRoleCommandValidator : AbstractValidator<RemoveRoleCommand>
{
    public RemoveRoleCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty().WithMessage("User ID must not be empty.");

        RuleFor(x => x.RoleId).NotEmpty().WithMessage("Role ID must not be empty.");
    }
}
