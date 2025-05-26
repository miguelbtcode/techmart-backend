using Microsoft.EntityFrameworkCore;
using TechMart.Auth.Domain.Roles.Entities;
using TechMart.Auth.Domain.Users.Entities;

namespace TechMart.Auth.Application.Abstractions.Data;

public interface IAuthDbContext
{
    DbSet<User> Users { get; }
    DbSet<Role> Roles { get; }
    DbSet<UserRole> UserRoles { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
