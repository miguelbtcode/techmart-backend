using Microsoft.EntityFrameworkCore;
using TechMart.Auth.Domain.Users.Entities;
using TechMart.Auth.Domain.Users.Repository;
using TechMart.Auth.Domain.Users.ValueObjects;

namespace TechMart.Auth.Infrastructure.Persistence.Repositories;

internal sealed class UserRepository : Repository<User, UserId>, IUserRepository
{
    private readonly AuthDbContext _context;

    public UserRepository(AuthDbContext context)
        : base(context)
    {
        _context = context;
    }

    public async Task<User?> GetByEmailAsync(
        Email email,
        CancellationToken cancellationToken = default
    )
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<bool> IsEmailUniqueAsync(
        Email email,
        UserId? excludeUserId = null,
        CancellationToken cancellationToken = default
    )
    {
        var query = _context.Users.Where(u => u.Email == email);

        if (excludeUserId != null)
        {
            query = query.Where(u => u.Id != excludeUserId);
        }

        return !await query.AnyAsync(cancellationToken);
    }

    public async Task<User?> GetByEmailWithRolesAsync(
        Email email,
        CancellationToken cancellationToken = default
    )
    {
        return await _context
            .Users.Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<User?> GetByIdWithRolesAsync(
        UserId id,
        CancellationToken cancellationToken = default
    )
    {
        return await _context
            .Users.Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }
}
