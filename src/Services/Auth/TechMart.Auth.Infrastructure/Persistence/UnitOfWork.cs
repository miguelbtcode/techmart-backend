using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using TechMart.Auth.Domain.Primitives;
using TechMart.Auth.Domain.Roles.Repository;
using TechMart.Auth.Domain.Users.Repository;
using TechMart.Auth.Infrastructure.Persistence.Repositories;

namespace TechMart.Auth.Infrastructure.Persistence;

internal sealed class UnitOfWork : IUnitOfWork
{
    private readonly AuthDbContext _context;
    private readonly Dictionary<Type, object> _repositories = new();
    private IDbContextTransaction? _transaction;

    private IUserRepository? _users;
    private IRoleRepository? _roles;
    private IUserRoleRepository? _userRoles;

    public UnitOfWork(AuthDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public IUserRepository Users => _users ??= new UserRepository(_context);
    public IRoleRepository Roles => _roles ??= new RoleRepository(_context);
    public IUserRoleRepository UserRoles => _userRoles ??= new UserRoleRepository(_context);

    public IRepository<TEntity, TId> Repository<TEntity, TId>()
        where TEntity : Entity<TId>
        where TId : notnull
    {
        var key = typeof(TEntity);

        if (_repositories.TryGetValue(key, out var repository))
        {
            return (IRepository<TEntity, TId>)repository;
        }

        var newRepository = new Repository<TEntity, TId>(_context);
        _repositories[key] = newRepository;

        return newRepository;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<Result> SaveChangesWithResultAsync(
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
        catch (DbUpdateException ex)
        {
            return Result.Failure(Error.Failure("Database.UpdateFailed", ex.Message));
        }
        catch (Exception ex)
        {
            return Result.Failure(Error.Failure("Database.UnknownError", ex.Message));
        }
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task DispatchDomainEventsAsync(CancellationToken cancellationToken = default)
    {
        var domainEntities = _context
            .ChangeTracker.Entries<IAggregateRoot>()
            .Where(x => x.Entity.GetDomainEvents().Any())
            .ToList();

        var domainEvents = domainEntities.SelectMany(x => x.Entity.GetDomainEvents()).ToList();

        domainEntities.ForEach(entity => entity.Entity.ClearDomainEvents());

        foreach (var domainEvent in domainEvents)
        {
            // Here you would publish the domain event using MediatR or similar
            // This is typically injected as IMediator or IPublisher
            // await _publisher.Publish(domainEvent, cancellationToken);
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
