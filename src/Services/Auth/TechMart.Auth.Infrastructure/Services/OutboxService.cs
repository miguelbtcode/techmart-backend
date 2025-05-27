using Microsoft.Extensions.Logging;
using TechMart.Auth.Application.Abstractions.Events;
using TechMart.Auth.Application.Services;
using TechMart.Auth.Domain.Primitives;

namespace TechMart.Auth.Infrastructure.Services;

public class OutboxService : IOutboxService
{
    private readonly IOutboxRepository _outboxRepository;
    private readonly IEventSerializer _eventSerializer;
    private readonly ILogger<OutboxService> _logger;

    public OutboxService(
        IOutboxRepository outboxRepository,
        IEventSerializer eventSerializer,
        ILogger<OutboxService> logger
    )
    {
        _outboxRepository = outboxRepository;
        _eventSerializer = eventSerializer;
        _logger = logger;
    }

    public async Task SaveDomainEventsAsync(
        IEnumerable<IDomainEvent> domainEvents,
        CancellationToken cancellationToken = default
    )
    {
        foreach (var domainEvent in domainEvents)
        {
            try
            {
                var eventType = domainEvent.GetType().Name;
                var eventData = _eventSerializer.Serialize(domainEvent);

                var outboxMessage = new OutboxMessage(domainEvent, eventType, eventData);
                await _outboxRepository.AddAsync(outboxMessage, cancellationToken);

                _logger.LogDebug(
                    "Domain event {EventType} with ID {EventId} saved to outbox",
                    eventType,
                    domainEvent.Id
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to save domain event {EventType} to outbox",
                    domainEvent.GetType().Name
                );
                throw;
            }
        }
    }
}
