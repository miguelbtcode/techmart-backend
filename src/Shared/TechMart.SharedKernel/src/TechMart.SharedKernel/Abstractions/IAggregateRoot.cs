namespace TechMart.SharedKernel.Abstractions;

/// <summary>
/// Marker interface for aggregate roots in Domain-Driven Design.
/// An aggregate root is the only member of its aggregate that outside objects are allowed to hold references to.
/// </summary>
public interface IAggregateRoot<TId> : IEntity<TId>
{
    /// <summary>
    /// Gets the collection of domain events that have been raised by this aggregate.
    /// </summary>
    IReadOnlyList<IDomainEvent> DomainEvents { get; }
    
    /// <summary>
    /// Adds a domain event to the aggregate.
    /// </summary>
    /// <param name="domainEvent">The domain event to add.</param>
    void AddDomainEvent(IDomainEvent domainEvent);
    
    /// <summary>
    /// Removes a domain event from the aggregate.
    /// </summary>
    /// <param name="domainEvent">The domain event to remove.</param>
    void RemoveDomainEvent(IDomainEvent domainEvent);
    
    /// <summary>
    /// Clears all domain events from the aggregate.
    /// </summary>
    void ClearDomainEvents();
}