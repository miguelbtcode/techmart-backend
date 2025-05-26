namespace TechMart.Auth.Domain.Primitives;

/// <summary>
/// Marker interface for aggregate roots
/// </summary>
public interface IAggregateRoot
{
    /// <summary>
    /// Gets all domain events that have been raised by this aggregate
    /// </summary>
    IReadOnlyCollection<IDomainEvent> GetDomainEvents();

    /// <summary>
    /// Clears all domain events from this aggregate
    /// </summary>
    void ClearDomainEvents();
}

public abstract class AggregateRoot<TId> : Entity<TId>, IAggregateRoot
    where TId : notnull
{
    private readonly List<IDomainEvent> _domainEvents = [];

    protected AggregateRoot(TId id)
        : base(id) { }

    protected AggregateRoot()
        : base() { }

    /// <summary>
    /// Gets all domain events that have been raised by this aggregate
    /// </summary>
    public IReadOnlyCollection<IDomainEvent> GetDomainEvents() => _domainEvents.AsReadOnly();

    /// <summary>
    /// Clears all domain events from this aggregate
    /// </summary>
    public void ClearDomainEvents() => _domainEvents.Clear();

    /// <summary>
    /// Raises a domain event
    /// </summary>
    /// <param name="domainEvent">The domain event to raise</param>
    protected void RaiseDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
}
