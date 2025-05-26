using TechMart.Auth.Domain.Primitives;

namespace TechMart.Auth.Domain.Roles.Events;

public sealed record UserRoleAssignedEvent(
    Guid UserId,
    Guid RoleId,
    string RoleName,
    Guid? AssignedBy,
    DateTime AssignedAt
) : DomainEventBase;
