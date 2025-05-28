using TechMart.Auth.Application.Messaging.Commands;

namespace TechMart.Auth.Application.Features.Roles.Commands.AssignRole;

public sealed record AssignRoleCommand(Guid UserId, Guid RoleId, Guid? AssignedBy = null)
    : ICommand;
