using MediatR;
using Microsoft.Extensions.Logging;
using TechMart.Product.Domain.Product.Events;
using TechMart.SharedKernel.Abstractions;

namespace TechMart.Product.Application.Features.Products.Events;

public class ProductDeletedEventHandler : INotificationHandler<ProductDeletedEvent>
{
    private readonly ILogger<ProductDeletedEventHandler> _logger;
    private readonly IEventPublisher _eventPublisher;

    public ProductDeletedEventHandler(
        ILogger<ProductDeletedEventHandler> logger,
        IEventPublisher eventPublisher)
    {
        _logger = logger;
        _eventPublisher = eventPublisher;
    }

    public async Task Handle(ProductDeletedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling ProductDeletedEvent for product: {ProductId} - {ProductName}", 
            notification.ProductId, notification.Name);

        try
        {
            // Publish integration event for other bounded contexts
            // This could trigger:
            // - Search index removal
            // - Cache invalidation
            // - Analytics events
            // - Related data cleanup
            
            var integrationEvent = new ProductDeletedIntegrationEvent(
                notification.ProductId,
                notification.Sku,
                notification.Name,
                notification.DeletedAt);

            await _eventPublisher.PublishIntegrationEventAsync(integrationEvent, cancellationToken);

            _logger.LogInformation("ProductDeletedEvent handled successfully for product: {ProductId}", notification.ProductId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling ProductDeletedEvent for product: {ProductId}", notification.ProductId);
        }
    }
}

public class ProductDeletedIntegrationEvent : IIntegrationEvent
{
    public Guid Id { get; }
    public DateTime OccurredOn { get; }
    public string EventType => nameof(ProductDeletedIntegrationEvent);
    public int Version => 1;

    public Guid ProductId { get; }
    public string Sku { get; }
    public string Name { get; }
    public DateTime DeletedAt { get; }

    public ProductDeletedIntegrationEvent(
        Guid productId,
        string sku,
        string name,
        DateTime deletedAt)
    {
        Id = Guid.NewGuid();
        ProductId = productId;
        Sku = sku;
        Name = name;
        DeletedAt = deletedAt;
        OccurredOn = deletedAt;
    }
}