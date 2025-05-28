using Microsoft.EntityFrameworkCore;
using TechMart.Auth.Domain.Primitives;
using TechMart.Auth.Domain.Roles.Entities;
using TechMart.Auth.Domain.Users.Entities;

namespace TechMart.Auth.Application.Contracts.Persistence;

/// <summary>
/// Application database context interface
/// Provides access to all DbSets without exposing EF Core implementation details
/// </summary>
public interface IApplicationDbContext
{
    /// <summary>
    /// Users DbSet
    /// </summary>
    DbSet<User> Users { get; }

    /// <summary>
    /// Roles DbSet
    /// </summary>
    DbSet<Role> Roles { get; }

    /// <summary>
    /// UserRoles DbSet
    /// </summary>
    DbSet<UserRole> UserRoles { get; }

    /// <summary>
    /// Outbox messages for event sourcing
    /// </summary>
    DbSet<OutboxMessage> OutboxMessages { get; }

    /// <summary>
    /// Save changes to the database
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Begin a database transaction
    /// </summary>
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Commit the current transaction
    /// </summary>
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Rollback the current transaction
    /// </summary>
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Execute raw SQL command
    /// </summary>
    Task<int> ExecuteSqlRawAsync(string sql, CancellationToken cancellationToken = default);

    /// <summary>
    /// Execute raw SQL command with parameters
    /// </summary>
    Task<int> ExecuteSqlRawAsync(
        string sql,
        IEnumerable<object> parameters,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Get DbSet for any entity type
    /// </summary>
    DbSet<TEntity> Set<TEntity>()
        where TEntity : class;
}
