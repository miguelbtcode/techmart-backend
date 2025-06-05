using MediatR;
using Microsoft.Extensions.Logging;
using TechMart.Product.Domain.Product.Events;
using TechMart.SharedKernel.Abstractions;

namespace TechMart.Product.Application.Features.Products.Events;

public class ProductCreatedEventHandler : INotificationHandler<ProductCreatedEvent>
{
    private readonly ILogger<ProductCreatedEventHandler> _logger;
    private readonly IEventPublisher _eventPublisher;

    public ProductCreatedEventHandler(
        ILogger<ProductCreatedEventHandler> logger,
        IEventPublisher eventPublisher)
    {
        _logger = logger;
        _eventPublisher = eventPublisher;
    }

    public async Task Handle(ProductCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling ProductCreatedEvent for product: {ProductId} - {ProductName}", 
            notification.ProductId, notification.Name);

        try
        {
            // Publish integration event for other bounded contexts
            // This could trigger:
            // - Search index updates
            // - Catalog service notifications
            // - Analytics events
            // - Cache invalidation
            
            // Example: Create integration event
            var integrationEvent = new ProductCreatedIntegrationEvent(
                notification.ProductId,
                notification.Sku,
                notification.Name,
                notification.Price,
                notification.BrandId,
                notification.CategoryId,
                notification.OccurredOn);

            await _eventPublisher.PublishIntegrationEventAsync(integrationEvent, cancellationToken);

            _logger.LogInformation("ProductCreatedEvent handled successfully for product: {ProductId}", notification.ProductId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling ProductCreatedEvent for product: {ProductId}", notification.ProductId);
            // Don't rethrow - event handling should not fail the main operation
        }
    }
}

// Integration Event for external systems
public class ProductCreatedIntegrationEvent : IIntegrationEvent
{
    public Guid Id { get; }
    public DateTime OccurredOn { get; }
    public string EventType => nameof(ProductCreatedIntegrationEvent);
    public int Version => 1;

    public Guid ProductId { get; }
    public string Sku { get; }
    public string Name { get; }
    public decimal Price { get; }
    public Guid BrandId { get; }
    public Guid CategoryId { get; }

    public ProductCreatedIntegrationEvent(
        Guid productId,
        string sku,
        string name,
        decimal price,
        Guid brandId,
        Guid categoryId,
        DateTime occurredOn)
    {
        Id = Guid.NewGuid();
        ProductId = productId;
        Sku = sku;
        Name = name;
        Price = price;
        BrandId = brandId;
        CategoryId = categoryId;
        OccurredOn = occurredOn;
    }
}