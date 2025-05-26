using TechMart.Auth.Domain.Primitives;
using TechMart.Auth.Domain.Roles.Constants;
using TechMart.Auth.Domain.Roles.Errors;
using TechMart.Auth.Domain.Roles.Events;
using TechMart.Auth.Domain.Roles.ValueObjects;
using TechMart.Auth.Domain.Users.Entities;
using TechMart.Auth.Domain.Users.ValueObjects;

namespace TechMart.Auth.Domain.Roles.Entities;

public sealed class UserRole : AggregateRoot<UserRoleId>
{
    private UserRole()
        : base() { }

    private UserRole(
        UserRoleId id,
        UserId userId,
        RoleId roleId,
        Guid? assignedBy,
        DateTime assignedAt
    )
        : base(id)
    {
        UserId = userId;
        RoleId = roleId;
        AssignedAt = assignedAt;
        CreatedAt = assignedAt;
        UpdatedAt = assignedAt;

        if (assignedBy.HasValue)
            SetCreatedBy(assignedBy.Value);
    }

    public UserId UserId { get; private set; }
    public RoleId RoleId { get; private set; }
    public DateTime AssignedAt { get; private set; }

    public User User { get; private set; } = null!;
    public Role Role { get; private set; } = null!;

    public static Result<UserRole> Create(
        UserId userId,
        RoleId roleId,
        Guid? assignedBy = null,
        bool isInitialAssignment = false
    )
    {
        if (!RoleConstants.RoleIdToName.TryGetValue(roleId, out var roleName))
            return RoleErrors.RoleNotFound(roleId);

        var assignedAt = DateTime.UtcNow;
        var userRoleId = UserRoleId.New();
        var userRole = new UserRole(userRoleId, userId, roleId, assignedBy, assignedAt);

        if (!isInitialAssignment)
        {
            userRole.RaiseDomainEvent(
                new UserRoleAssignedEvent(
                    userId.Value,
                    roleId.Value,
                    roleName,
                    assignedBy,
                    assignedAt
                )
            );
        }

        return userRole;
    }

    /// <summary>
    /// Removes this user role assignment
    /// </summary>
    public void Remove(Guid? removedBy = null)
    {
        var roleName =
            Role?.Name ?? RoleConstants.RoleIdToName.GetValueOrDefault(RoleId.Value, "Unknown");

        RaiseDomainEvent(
            new UserRoleRemovedEvent(
                UserId.Value,
                RoleId.Value,
                roleName,
                removedBy,
                DateTime.UtcNow
            )
        );
    }
}
