using FluentValidation;

namespace TechMart.Auth.Application.Features.Roles.Commands.AssignRole;

internal sealed class AssignRoleCommandValidator : AbstractValidator<AssignRoleCommand>
{
    public AssignRoleCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty().WithMessage("User ID must not be empty.");

        RuleFor(x => x.RoleId).NotEmpty().WithMessage("Role ID must not be empty.");
    }
}
