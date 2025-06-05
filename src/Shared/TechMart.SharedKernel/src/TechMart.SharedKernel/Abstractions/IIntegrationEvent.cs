namespace TechMart.SharedKernel.Abstractions;

/// <summary>
/// Marker interface for integration events that are published to external systems.
/// Integration events represent something that happened in one bounded context 
/// that other bounded contexts might be interested in.
/// </summary>
public interface IIntegrationEvent
{
    /// <summary>
    /// Gets the unique identifier of the event.
    /// </summary>
    Guid Id { get; }
    
    /// <summary>
    /// Gets the date and time when the event occurred.
    /// </summary>
    DateTime OccurredOn { get; }
    
    /// <summary>
    /// Gets the event type name for routing purposes.
    /// </summary>
    string EventType { get; }
    
    /// <summary>
    /// Gets the version of the event schema.
    /// </summary>
    int Version { get; }
}