namespace TechMart.Auth.Domain.Primitives;

/// <summary>
/// Marker interface for domain events
/// </summary>
public interface IDomainEvent
{
    /// <summary>
    /// Unique identifier for the event
    /// </summary>
    Guid Id { get; }

    /// <summary>
    /// When the event occurred (UTC)
    /// </summary>
    DateTime OccurredAt { get; }

    /// <summary>
    /// Name of the event for identification
    /// </summary>
    string EventName { get; }
}

public abstract record DomainEventBase : IDomainEvent
{
    protected DomainEventBase()
    {
        Id = Guid.NewGuid();
        OccurredAt = DateTime.UtcNow;
        EventName = GetType().Name;
    }

    public Guid Id { get; init; }
    public DateTime OccurredAt { get; init; }
    public string EventName { get; init; }
}
