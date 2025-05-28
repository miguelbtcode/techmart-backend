using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using TechMart.Auth.Application.Abstractions.Events;
using TechMart.Auth.Domain.Primitives;
using TechMart.Auth.Domain.Roles.Events;
using TechMart.Auth.Domain.Users.Events;

namespace TechMart.Auth.Infrastructure.Events;

/// <summary>
/// JSON-based implementation of event serializer
/// Handles serialization/deserialization of domain events with robust error handling
/// </summary>
public sealed class JsonEventSerializer : IEventSerializer
{
    private readonly JsonSerializerOptions _serializerOptions;
    private readonly JsonSerializerOptions _metadataOptions;
    private readonly Dictionary<string, Type> _eventTypes;
    private readonly ILogger<JsonEventSerializer> _logger;

    private const string SERIALIZATION_VERSION = "1.0";

    public JsonEventSerializer(ILogger<JsonEventSerializer> logger)
    {
        _logger = logger;

        // Configure JSON serialization options
        _serializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false, // Compact format for storage
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };

        // Options for metadata serialization (more readable)
        _metadataOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true, // Pretty format for external systems
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };

        // Register all known domain event types
        _eventTypes = RegisterEventTypes();

        _logger.LogDebug(
            "JsonEventSerializer initialized with {EventTypeCount} registered event types",
            _eventTypes.Count
        );
    }

    public string Serialize<T>(T domainEvent)
        where T : IDomainEvent
    {
        try
        {
            ArgumentNullException.ThrowIfNull(domainEvent);

            var serialized = JsonSerializer.Serialize(domainEvent, _serializerOptions);

            _logger.LogTrace(
                "Serialized event {EventType} with ID {EventId}",
                typeof(T).Name,
                domainEvent.Id
            );

            return serialized;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to serialize event {EventType} with ID {EventId}",
                typeof(T).Name,
                domainEvent?.Id
            );
            throw new InvalidOperationException(
                $"Failed to serialize event of type {typeof(T).Name}",
                ex
            );
        }
    }

    public IDomainEvent Deserialize(string eventType, string eventData)
    {
        try
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(eventType);
            ArgumentException.ThrowIfNullOrWhiteSpace(eventData);

            if (!_eventTypes.TryGetValue(eventType, out var type))
            {
                throw new ArgumentException(
                    $"Unknown event type: {eventType}. Supported types: {string.Join(", ", _eventTypes.Keys)}"
                );
            }

            var deserializedEvent = JsonSerializer.Deserialize(eventData, type, _serializerOptions);
            if (deserializedEvent == null)
            {
                throw new InvalidOperationException(
                    $"Deserialization resulted in null for event type {eventType}"
                );
            }

            _logger.LogTrace("Deserialized event {EventType}", eventType);

            return (IDomainEvent)deserializedEvent;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to deserialize event {EventType}. Data: {EventData}",
                eventType,
                eventData
            );
            throw new InvalidOperationException(
                $"Failed to deserialize event of type {eventType}",
                ex
            );
        }
    }

    public T Deserialize<T>(string eventData)
        where T : IDomainEvent
    {
        try
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(eventData);

            var deserializedEvent = JsonSerializer.Deserialize<T>(eventData, _serializerOptions);
            if (deserializedEvent == null)
            {
                throw new InvalidOperationException(
                    $"Deserialization resulted in null for event type {typeof(T).Name}"
                );
            }

            _logger.LogTrace("Deserialized event {EventType}", typeof(T).Name);

            return deserializedEvent;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to deserialize event {EventType}. Data: {EventData}",
                typeof(T).Name,
                eventData
            );
            throw new InvalidOperationException(
                $"Failed to deserialize event of type {typeof(T).Name}",
                ex
            );
        }
    }

    public bool SupportsEventType(string eventType)
    {
        return !string.IsNullOrWhiteSpace(eventType) && _eventTypes.ContainsKey(eventType);
    }

    public IReadOnlyCollection<string> GetSupportedEventTypes()
    {
        return _eventTypes.Keys.ToList().AsReadOnly();
    }

    public string SerializeWithMetadata<T>(T domainEvent, bool includeMetadata = true)
        where T : IDomainEvent
    {
        try
        {
            ArgumentNullException.ThrowIfNull(domainEvent);

            if (!includeMetadata)
            {
                return Serialize(domainEvent);
            }

            // Create envelope with metadata for external systems
            var envelope = new EventEnvelope<T>
            {
                EventId = domainEvent.Id,
                EventType = typeof(T).Name,
                EventName = domainEvent.EventName,
                OccurredAt = domainEvent.OccurredAt,
                Data = domainEvent,
                Metadata = new EventMetadata
                {
                    SerializationVersion = SERIALIZATION_VERSION,
                    Source = "TechMart.Auth",
                    CorrelationId = Guid.NewGuid().ToString(),
                    ContentType = "application/json",
                    SchemaVersion = "1.0",
                },
            };

            var serialized = JsonSerializer.Serialize(envelope, _metadataOptions);

            _logger.LogTrace("Serialized event {EventType} with metadata", typeof(T).Name);

            return serialized;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to serialize event {EventType} with metadata",
                typeof(T).Name
            );
            throw new InvalidOperationException(
                $"Failed to serialize event of type {typeof(T).Name} with metadata",
                ex
            );
        }
    }

    public bool IsValidEventData(string eventType, string eventData)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(eventType) || string.IsNullOrWhiteSpace(eventData))
            {
                return false;
            }

            if (!_eventTypes.TryGetValue(eventType, out var type))
            {
                return false;
            }

            // Try to deserialize - if it succeeds, it's valid
            var result = JsonSerializer.Deserialize(eventData, type, _serializerOptions);
            return result != null;
        }
        catch (Exception ex)
        {
            _logger.LogTrace(ex, "Event data validation failed for {EventType}", eventType);
            return false;
        }
    }

    public string GetSerializationVersion() => SERIALIZATION_VERSION;

    /// <summary>
    /// Register all known domain event types for deserialization
    /// Add new event types here when you create them
    /// </summary>
    private Dictionary<string, Type> RegisterEventTypes()
    {
        var eventTypes = new Dictionary<string, Type>
        {
            // User Events
            [nameof(UserCreatedEvent)] = typeof(UserCreatedEvent),
            [nameof(UserEmailConfirmedEvent)] = typeof(UserEmailConfirmedEvent),
            [nameof(UserLoggedInEvent)] = typeof(UserLoggedInEvent),
            [nameof(UserPasswordChangedEvent)] = typeof(UserPasswordChangedEvent),
            [nameof(UserStatusChangedEvent)] = typeof(UserStatusChangedEvent),

            // Role Events
            [nameof(UserRoleAssignedEvent)] = typeof(UserRoleAssignedEvent),
            [nameof(UserRoleRemovedEvent)] = typeof(UserRoleRemovedEvent),
        };

        // Validate that all types implement IDomainEvent
        foreach (var kvp in eventTypes.ToList())
        {
            if (!typeof(IDomainEvent).IsAssignableFrom(kvp.Value))
            {
                _logger.LogWarning(
                    "Event type {EventType} does not implement IDomainEvent, removing from registry",
                    kvp.Key
                );
                eventTypes.Remove(kvp.Key);
            }
        }

        return eventTypes;
    }
}

/// <summary>
/// Envelope for events with metadata (for external systems)
/// </summary>
internal sealed class EventEnvelope<T>
    where T : IDomainEvent
{
    public Guid EventId { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string EventName { get; set; } = string.Empty;
    public DateTime OccurredAt { get; set; }
    public T Data { get; set; } = default!;
    public EventMetadata Metadata { get; set; } = new();
}

/// <summary>
/// Metadata attached to events for external systems
/// </summary>
internal sealed class EventMetadata
{
    public string SerializationVersion { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public string CorrelationId { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public string SchemaVersion { get; set; } = string.Empty;
    public Dictionary<string, string> CustomProperties { get; set; } = new();
}
