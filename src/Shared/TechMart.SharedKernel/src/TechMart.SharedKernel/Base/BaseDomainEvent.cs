using TechMart.SharedKernel.Abstractions;

namespace TechMart.SharedKernel.Base;

/// <summary>
/// Base class for domain events.
/// Domain events represent something important that happened in the domain.
/// </summary>
public abstract class BaseDomainEvent : IDomainEvent
{
    /// <summary>
    /// Gets the unique identifier of the event.
    /// </summary>
    public Guid EventId { get; }

    /// <summary>
    /// Gets the date and time when the event occurred.
    /// </summary>
    public DateTime OccurredOn { get; }

    /// <summary>
    /// Gets the type name of the event.
    /// </summary>
    public string EventType => GetType().Name;

    protected BaseDomainEvent()
    {
        EventId = Guid.NewGuid();
        OccurredOn = DateTime.UtcNow;
    }

    protected BaseDomainEvent(DateTime occurredOn)
    {
        EventId = Guid.NewGuid();
        OccurredOn = occurredOn;
    }

    protected BaseDomainEvent(Guid eventId, DateTime occurredOn)
    {
        EventId = eventId;
        OccurredOn = occurredOn;
    }

    public override bool Equals(object? obj)
    {
        if (obj is not BaseDomainEvent other)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        if (GetType() != other.GetType())
            return false;

        return EventId == other.EventId;
    }

    public override int GetHashCode()
    {
        return EventId.GetHashCode();
    }

    public override string ToString()
    {
        return $"{EventType} - {EventId} - {OccurredOn:yyyy-MM-dd HH:mm:ss} UTC";
    }
}