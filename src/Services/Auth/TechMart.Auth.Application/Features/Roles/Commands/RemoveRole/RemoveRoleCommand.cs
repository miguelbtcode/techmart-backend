using TechMart.Auth.Application.Messaging.Commands;

namespace TechMart.Auth.Application.Features.Roles.Commands.RemoveRole;

public sealed record RemoveRoleCommand(Guid UserId, Guid RoleId, Guid? RemovedBy = null) : ICommand;
