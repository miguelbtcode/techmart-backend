using TechMart.Auth.Domain.Primitives;
using TechMart.Auth.Domain.Roles.Entities;
using TechMart.Auth.Domain.Roles.ValueObjects;

namespace TechMart.Auth.Domain.Roles.Repository;

/// <summary>
/// Role-specific repository operations
/// </summary>
public interface IRoleRepository : IRepository<Role, RoleId>
{
    Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
}
