using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TechMart.Auth.Application.Common.Events;
using TechMart.Auth.Application.Contracts.Infrastructure;
using TechMart.Auth.Domain.Primitives;
using TechMart.Auth.Infrastructure.Services;

namespace TechMart.Auth.Infrastructure.Events;

/// <summary>
/// Hybrid domain event dispatcher that processes critical events immediately
/// and defers non-critical events to background processing via Outbox pattern
/// </summary>
public sealed class HybridDomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IServiceProvider _serviceProvider;
    private readonly OutboxService _outboxService;
    private readonly ILogger<HybridDomainEventDispatcher> _logger;

    public HybridDomainEventDispatcher(
        IServiceProvider serviceProvider,
        IOutboxService outboxService,
        ILogger<HybridDomainEventDispatcher> logger
    )
    {
        _serviceProvider = serviceProvider;
        _outboxService = outboxService;
        _logger = logger;
    }

    public async Task DispatchAsync<TDomainEvent>(
        TDomainEvent domainEvent,
        CancellationToken cancellationToken = default
    )
        where TDomainEvent : IDomainEvent
    {
        await DispatchAsync(new[] { domainEvent }, cancellationToken);
    }

    public async Task DispatchAsync(
        IEnumerable<IDomainEvent> domainEvents,
        CancellationToken cancellationToken = default
    )
    {
        var events = domainEvents.ToList();
        if (!events.Any())
        {
            return;
        }

        _logger.LogDebug("Dispatching {EventCount} domain events", events.Count);

        // 1. Separate critical from deferred events
        var criticalEvents = events.OfType<ICriticalDomainEvent>().Cast<IDomainEvent>().ToList();
        var deferredEvents = events.OfType<IDeferredDomainEvent>().Cast<IDomainEvent>().ToList();

        // Events that are neither critical nor deferred go to outbox by default
        var regularEvents = events.Except(criticalEvents).Except(deferredEvents).ToList();

        // 2. Process critical events immediately
        if (criticalEvents.Any())
        {
            _logger.LogDebug(
                "Processing {CriticalEventCount} critical events immediately",
                criticalEvents.Count
            );
            await ProcessCriticalEventsAsync(criticalEvents, cancellationToken);
        }

        // 3. Save non-critical events to outbox for background processing
        var outboxEvents = deferredEvents.Concat(regularEvents).ToList();
        if (outboxEvents.Any())
        {
            _logger.LogDebug(
                "Saving {OutboxEventCount} events to outbox for background processing",
                outboxEvents.Count
            );
            await _outboxService.SaveDomainEventsAsync(outboxEvents, cancellationToken);
        }

        _logger.LogDebug("Domain event dispatching completed");
    }

    private async Task ProcessCriticalEventsAsync(
        IList<IDomainEvent> criticalEvents,
        CancellationToken cancellationToken
    )
    {
        try
        {
            // Sort critical events by priority
            var sortedEvents = criticalEvents
                .OfType<ICriticalDomainEvent>()
                .OrderBy(e => e.Priority)
                .Cast<IDomainEvent>()
                .ToList();

            // Process each critical event
            foreach (var domainEvent in sortedEvents)
            {
                await ProcessSingleEventAsync(domainEvent, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to process critical domain events. Moving to outbox for retry"
            );

            // If critical events fail, move them to outbox for retry
            // This ensures eventual consistency even for critical events
            await _outboxService.SaveDomainEventsAsync(criticalEvents, cancellationToken);
        }
    }

    private async Task ProcessSingleEventAsync(
        IDomainEvent domainEvent,
        CancellationToken cancellationToken
    )
    {
        var eventType = domainEvent.GetType();
        var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(eventType);

        // Get all handlers for this event type
        var handlers = _serviceProvider.GetServices(handlerType).ToList();

        if (!handlers.Any())
        {
            _logger.LogWarning("No handlers found for critical event {EventType}", eventType.Name);
            return;
        }

        _logger.LogDebug(
            "Processing critical event {EventType} with {HandlerCount} handlers",
            eventType.Name,
            handlers.Count
        );

        // Execute all handlers in parallel for better performance
        var tasks = handlers.Select(async handler =>
        {
            try
            {
                var method = handler.GetType().GetMethod("Handle");
                if (method != null)
                {
                    var task = (Task)
                        method.Invoke(handler, new object[] { domainEvent, cancellationToken });
                    await task;

                    _logger.LogDebug(
                        "Critical event {EventType} processed successfully by {HandlerType}",
                        eventType.Name,
                        handler.GetType().Name
                    );
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Critical event {EventType} failed in handler {HandlerType}",
                    eventType.Name,
                    handler.GetType().Name
                );

                // Re-throw to ensure the event gets moved to outbox
                throw;
            }
        });

        await Task.WhenAll(tasks);
    }
}
