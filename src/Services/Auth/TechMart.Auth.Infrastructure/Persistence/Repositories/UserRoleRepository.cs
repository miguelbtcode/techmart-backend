using Microsoft.EntityFrameworkCore;
using TechMart.Auth.Domain.Roles.Entities;
using TechMart.Auth.Domain.Roles.Repository;
using TechMart.Auth.Domain.Roles.ValueObjects;
using TechMart.Auth.Domain.Users.ValueObjects;

namespace TechMart.Auth.Infrastructure.Persistence.Repositories;

internal sealed class UserRoleRepository : Repository<UserRole, UserRoleId>, IUserRoleRepository
{
    private readonly AuthDbContext _context;

    public UserRoleRepository(AuthDbContext context)
        : base(context)
    {
        _context = context;
    }

    public async Task<UserRole?> GetUserRoleAsync(
        UserId userId,
        RoleId roleId,
        CancellationToken cancellationToken = default
    )
    {
        return await _context
            .UserRoles.Include(ur => ur.Role)
            .FirstOrDefaultAsync(
                ur => ur.UserId == userId && ur.RoleId == roleId,
                cancellationToken
            );
    }

    public async Task<IEnumerable<UserRole>> GetUserRolesAsync(
        UserId userId,
        CancellationToken cancellationToken = default
    )
    {
        return await _context
            .UserRoles.Include(ur => ur.Role)
            .Where(ur => ur.UserId == userId)
            .OrderBy(ur => ur.Role!.HierarchyLevel)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(
        UserId userId,
        RoleId roleId,
        CancellationToken cancellationToken = default
    )
    {
        return await _context.UserRoles.AnyAsync(
            ur => ur.UserId == userId && ur.RoleId == roleId,
            cancellationToken
        );
    }
}
