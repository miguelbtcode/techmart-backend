using System.Text.Json;
using TechMart.Auth.Application.Abstractions.Events;
using TechMart.Auth.Domain.Primitives;
using TechMart.Auth.Domain.Roles.Events;
using TechMart.Auth.Domain.Users.Events;

namespace TechMart.Auth.Infrastructure.Events;

public class JsonEventSerializer : IEventSerializer
{
    private readonly JsonSerializerOptions _options;
    private readonly Dictionary<string, Type> _eventTypes;

    public JsonEventSerializer()
    {
        _options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false,
        };

        // Register all domain events
        _eventTypes = new Dictionary<string, Type>
        {
            [nameof(UserRegisteredEvent)] = typeof(UserRegisteredEvent),
            [nameof(UserEmailConfirmedEvent)] = typeof(UserEmailConfirmedEvent),
            [nameof(UserLoggedInEvent)] = typeof(UserLoggedInEvent),
            [nameof(UserPasswordChangedEvent)] = typeof(UserPasswordChangedEvent),
            [nameof(UserStatusChangedEvent)] = typeof(UserStatusChangedEvent),
            [nameof(UserRoleAssignedEvent)] = typeof(UserRoleAssignedEvent),
            [nameof(UserRoleRemovedEvent)] = typeof(UserRoleRemovedEvent),
        };
    }

    public string Serialize<T>(T @event)
        where T : IDomainEvent
    {
        return JsonSerializer.Serialize(@event, _options);
    }

    public IDomainEvent Deserialize(string eventType, string eventData)
    {
        if (!_eventTypes.TryGetValue(eventType, out var type))
            throw new ArgumentException($"Unknown event type: {eventType}");

        var eventObject = JsonSerializer.Deserialize(eventData, type, _options);
        return (IDomainEvent)eventObject!;
    }
}
