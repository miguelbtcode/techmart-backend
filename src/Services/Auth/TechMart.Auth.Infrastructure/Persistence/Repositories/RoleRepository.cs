using Microsoft.EntityFrameworkCore;
using TechMart.Auth.Domain.Roles.Entities;
using TechMart.Auth.Domain.Roles.Repository;
using TechMart.Auth.Domain.Roles.ValueObjects;

namespace TechMart.Auth.Infrastructure.Persistence.Repositories;

internal sealed class RoleRepository : Repository<Role, RoleId>, IRoleRepository
{
    private readonly AuthDbContext _context;

    public RoleRepository(AuthDbContext context)
        : base(context)
    {
        _context = context;
    }

    public async Task<Role?> GetByNameAsync(
        string name,
        CancellationToken cancellationToken = default
    )
    {
        return await _context.Roles.FirstOrDefaultAsync(r => r.Name == name, cancellationToken);
    }
}
