using TechMart.Auth.Domain.Primitives;

namespace TechMart.Auth.Application.Abstractions.Events;

/// <summary>
/// Service for serializing and deserializing domain events
/// Used for storing events in outbox and sending to external systems
/// </summary>
public interface IEventSerializer
{
    /// <summary>
    /// Serialize a domain event to string format (typically JSON)
    /// </summary>
    /// <typeparam name="T">Type of domain event</typeparam>
    /// <param name="domainEvent">The domain event to serialize</param>
    /// <returns>Serialized string representation of the event</returns>
    string Serialize<T>(T domainEvent)
        where T : IDomainEvent;

    /// <summary>
    /// Deserialize a domain event from string format
    /// </summary>
    /// <param name="eventType">The type name of the event (e.g., "UserRegisteredEvent")</param>
    /// <param name="eventData">The serialized event data</param>
    /// <returns>Deserialized domain event</returns>
    IDomainEvent Deserialize(string eventType, string eventData);

    /// <summary>
    /// Deserialize a domain event to a specific type
    /// </summary>
    /// <typeparam name="T">Expected type of the domain event</typeparam>
    /// <param name="eventData">The serialized event data</param>
    /// <returns>Deserialized domain event of specified type</returns>
    T Deserialize<T>(string eventData)
        where T : IDomainEvent;

    /// <summary>
    /// Check if the serializer supports a specific event type
    /// </summary>
    /// <param name="eventType">The event type name to check</param>
    /// <returns>True if the event type is supported, false otherwise</returns>
    bool SupportsEventType(string eventType);

    /// <summary>
    /// Get all supported event types
    /// </summary>
    /// <returns>Collection of supported event type names</returns>
    IReadOnlyCollection<string> GetSupportedEventTypes();

    /// <summary>
    /// Serialize event with metadata for external systems (Kafka, etc.)
    /// </summary>
    /// <param name="domainEvent">The domain event to serialize</param>
    /// <param name="includeMetadata">Whether to include additional metadata</param>
    /// <returns>Serialized event with optional metadata</returns>
    string SerializeWithMetadata<T>(T domainEvent, bool includeMetadata = true)
        where T : IDomainEvent;

    /// <summary>
    /// Validate that event data can be deserialized without errors
    /// </summary>
    /// <param name="eventType">The event type name</param>
    /// <param name="eventData">The serialized event data</param>
    /// <returns>True if valid, false otherwise</returns>
    bool IsValidEventData(string eventType, string eventData);

    /// <summary>
    /// Get the version of the serialization format
    /// Used for backward compatibility when event schemas change
    /// </summary>
    /// <returns>Serialization format version</returns>
    string GetSerializationVersion();
}
