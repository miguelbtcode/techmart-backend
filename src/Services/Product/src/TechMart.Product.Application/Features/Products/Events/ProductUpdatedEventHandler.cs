using MediatR;
using Microsoft.Extensions.Logging;
using TechMart.Product.Domain.Product.Events;
using TechMart.SharedKernel.Abstractions;

namespace TechMart.Product.Application.Features.Products.Events;

public class ProductUpdatedEventHandler : INotificationHandler<ProductUpdatedEvent>
{
    private readonly ILogger<ProductUpdatedEventHandler> _logger;
    private readonly IEventPublisher _eventPublisher;

    public ProductUpdatedEventHandler(
        ILogger<ProductUpdatedEventHandler> logger,
        IEventPublisher eventPublisher)
    {
        _logger = logger;
        _eventPublisher = eventPublisher;
    }

    public async Task Handle(ProductUpdatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling ProductUpdatedEvent for product: {ProductId} - {ProductName}", 
            notification.ProductId, notification.Name);

        try
        {
            // Publish integration event for other bounded contexts
            var integrationEvent = new ProductUpdatedIntegrationEvent(
                notification.ProductId,
                notification.Name,
                notification.Description,
                notification.OccurredOn);

            await _eventPublisher.PublishIntegrationEventAsync(integrationEvent, cancellationToken);

            _logger.LogInformation("ProductUpdatedEvent handled successfully for product: {ProductId}", notification.ProductId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling ProductUpdatedEvent for product: {ProductId}", notification.ProductId);
        }
    }
}

public class ProductUpdatedIntegrationEvent : IIntegrationEvent
{
    public Guid Id { get; }
    public DateTime OccurredOn { get; }
    public string EventType => nameof(ProductUpdatedIntegrationEvent);
    public int Version => 1;

    public Guid ProductId { get; }
    public string Name { get; }
    public string Description { get; }

    public ProductUpdatedIntegrationEvent(
        Guid productId,
        string name,
        string description,
        DateTime occurredOn)
    {
        Id = Guid.NewGuid();
        ProductId = productId;
        Name = name;
        Description = description;
        OccurredOn = occurredOn;
    }
}