using TechMart.Auth.Domain.Roles.Repository;
using TechMart.Auth.Domain.Users.Repository;

namespace TechMart.Auth.Domain.Primitives;

/// <summary>
/// Unit of Work pattern interface for managing transactions
/// </summary>
public interface IUnitOfWork : IDisposable
{
    // Repository access actualizado
    IRepository<TEntity, TId> Repository<TEntity, TId>()
        where TEntity : Entity<TId> // Actualizado
        where TId : notnull;

    // Métodos específicos para acceder a repositorios tipados (opcional pero útil)
    IUserRepository Users { get; }
    IRoleRepository Roles { get; }
    IUserRoleRepository UserRoles { get; }

    // Transaction management
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<Result> SaveChangesWithResultAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);

    // Domain events
    Task DispatchDomainEventsAsync(CancellationToken cancellationToken = default);
}
