using TechMart.Auth.Domain.Primitives;

namespace TechMart.Auth.Domain.Roles.Events;

public sealed record UserRoleRemovedEvent(
    Guid UserId,
    Guid RoleId,
    string RoleName,
    Guid? RemovedBy,
    DateTime RemovedAt
) : DomainEventBase;
