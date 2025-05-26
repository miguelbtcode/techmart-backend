using TechMart.Auth.Domain.Primitives;
using TechMart.Auth.Domain.Roles.Entities;
using TechMart.Auth.Domain.Roles.ValueObjects;
using TechMart.Auth.Domain.Users.ValueObjects;

namespace TechMart.Auth.Domain.Roles.Repository;

/// <summary>
/// Repository for UserRole junction entity
/// </summary>
public interface IUserRoleRepository : IRepository<UserRole, UserRoleId>
{
    Task<UserRole?> GetUserRoleAsync(
        UserId userId,
        RoleId roleId,
        CancellationToken cancellationToken = default
    );
    Task<IEnumerable<UserRole>> GetUserRolesAsync(
        UserId userId,
        CancellationToken cancellationToken = default
    );
    Task<bool> ExistsAsync(
        UserId userId,
        RoleId roleId,
        CancellationToken cancellationToken = default
    );
}
