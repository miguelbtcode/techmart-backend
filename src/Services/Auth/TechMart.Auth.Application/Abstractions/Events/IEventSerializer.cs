using TechMart.Auth.Domain.Primitives;

namespace TechMart.Auth.Application.Abstractions.Events;

public interface IEventSerializer
{
    string Serialize<T>(T @event)
        where T : IDomainEvent;
    IDomainEvent Deserialize(string eventType, string eventData);
}
