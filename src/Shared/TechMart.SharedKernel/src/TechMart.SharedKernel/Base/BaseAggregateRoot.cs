using TechMart.SharedKernel.Abstractions;

namespace TechMart.SharedKernel.Base;

/// <summary>
/// Base class for aggregate roots with Guid identifiers.
/// </summary>
public abstract class BaseAggregateRoot : BaseAggregateRoot<Guid>
{
    protected BaseAggregateRoot() : base()
    {
    }

    protected BaseAggregateRoot(Guid id) : base(id)
    {
    }
}

/// <summary>
/// Base class for aggregate roots with custom identifier types.
/// Aggregate roots are the only objects that external objects can hold references to.
/// </summary>
/// <typeparam name="TId">The type of the entity identifier.</typeparam>
public abstract class BaseAggregateRoot<TId> : BaseEntity<TId>, IAggregateRoot<TId>
{
    private readonly List<IDomainEvent> _domainEvents = [];

    /// <summary>
    /// Gets the collection of domain events that have been raised by this aggregate.
    /// </summary>
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected BaseAggregateRoot() : base()
    {
    }

    protected BaseAggregateRoot(TId id) : base(id)
    {
    }

    /// <summary>
    /// Adds a domain event to the aggregate.
    /// </summary>
    /// <param name="domainEvent">The domain event to add.</param>
    public void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    /// <summary>
    /// Removes a domain event from the aggregate.
    /// </summary>
    /// <param name="domainEvent">The domain event to remove.</param>
    public void RemoveDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Remove(domainEvent);
    }

    /// <summary>
    /// Clears all domain events from the aggregate.
    /// </summary>
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    /// <summary>
    /// Adds a domain event to the aggregate. Protected method for derived classes.
    /// </summary>
    /// <param name="domainEvent">The domain event to add.</param>
    protected void RaiseDomainEvent(IDomainEvent domainEvent)
    {
        AddDomainEvent(domainEvent);
    }
}