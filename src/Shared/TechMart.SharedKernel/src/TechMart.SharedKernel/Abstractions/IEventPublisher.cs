namespace TechMart.SharedKernel.Abstractions;

/// <summary>
/// Service for publishing events to external systems or other bounded contexts.
/// </summary>
public interface IEventPublisher
{
    /// <summary>
    /// Publishes a domain event within the current bounded context.
    /// </summary>
    /// <param name="domainEvent">The domain event to publish.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task PublishDomainEventAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Publishes multiple domain events within the current bounded context.
    /// </summary>
    /// <param name="domainEvents">The domain events to publish.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task PublishDomainEventsAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Publishes an integration event to external systems or other bounded contexts.
    /// </summary>
    /// <param name="integrationEvent">The integration event to publish.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task PublishIntegrationEventAsync(IIntegrationEvent integrationEvent, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Publishes multiple integration events to external systems or other bounded contexts.
    /// </summary>
    /// <param name="integrationEvents">The integration events to publish.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task PublishIntegrationEventsAsync(IEnumerable<IIntegrationEvent> integrationEvents, CancellationToken cancellationToken = default);
}