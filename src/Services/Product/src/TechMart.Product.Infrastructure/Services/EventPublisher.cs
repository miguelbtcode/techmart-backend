using Microsoft.Extensions.Logging;
using TechMart.SharedKernel.Abstractions;

namespace TechMart.Product.Infrastructure.Services;

public class EventPublisher : IEventPublisher
{
    private readonly ILogger<EventPublisher> _logger;

    public EventPublisher(ILogger<EventPublisher> logger)
    {
        _logger = logger;
    }

    public Task PublishIntegrationEventAsync<T>(T integrationEvent, CancellationToken cancellationToken = default) 
        where T : IIntegrationEvent
    {
        // Por ahora solo logueamos el evento
        // En el futuro aquí puedes integrar con un message broker (RabbitMQ, Azure Service Bus, etc.)
        _logger.LogInformation("Publishing integration event: {EventType} with ID: {EventId}", 
            integrationEvent.EventType, integrationEvent.Id);

        return Task.CompletedTask;
    }

    public Task PublishDomainEventAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }

    public Task PublishDomainEventsAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }

    public Task PublishIntegrationEventAsync(IIntegrationEvent integrationEvent,
        CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }

    public Task PublishIntegrationEventsAsync(IEnumerable<IIntegrationEvent> integrationEvents,
        CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }
}