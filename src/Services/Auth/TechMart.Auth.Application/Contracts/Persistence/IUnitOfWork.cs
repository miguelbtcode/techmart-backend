using TechMart.Auth.Domain.Primitives;
using TechMart.Auth.Domain.Roles.Repository;
using TechMart.Auth.Domain.Users.Repository;

namespace TechMart.Auth.Application.Contracts.Persistence;

/// <summary>
/// Unit of Work pattern interface for managing transactions and repositories
/// This is a reference to the Domain's IUnitOfWork but accessible from Application layer
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Generic repository access for any entity
    /// </summary>
    IRepository<TEntity, TId> Repository<TEntity, TId>()
        where TEntity : Entity<TId>
        where TId : notnull;

    /// <summary>
    /// Specific repository for Users
    /// </summary>
    IUserRepository Users { get; }

    /// <summary>
    /// Specific repository for Roles
    /// </summary>
    IRoleRepository Roles { get; }

    /// <summary>
    /// Specific repository for UserRoles
    /// </summary>
    IUserRoleRepository UserRoles { get; }

    /// <summary>
    /// Outbox repository for domain events
    /// </summary>
    IOutboxRepository OutboxMessages { get; }

    /// <summary>
    /// Save all changes to the database
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Save changes and return Result pattern
    /// </summary>
    Task<Result> SaveChangesWithResultAsync(CancellationToken cancellationToken = default);

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
    /// Dispatch and publish domain events
    /// </summary>
    Task DispatchDomainEventsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if there are pending changes
    /// </summary>
    bool HasChanges();

    /// <summary>
    /// Clear all tracked entities
    /// </summary>
    void ClearChangeTracker();
}
