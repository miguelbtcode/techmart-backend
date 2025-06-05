using MediatR;
using Microsoft.Extensions.Logging;
using TechMart.Product.Application.Contracts.Infrastructure;
using TechMart.Product.Domain.Inventory.Enums;
using TechMart.Product.Domain.Inventory.Events;
using TechMart.SharedKernel.Abstractions;

namespace TechMart.Product.Application.Features.Inventory.Events;

/// <summary>
/// Handles StockUpdatedEvent to trigger notifications and other side effects.
/// </summary>
public class StockUpdatedEventHandler : INotificationHandler<StockUpdatedEvent>
{
    private readonly ILogger<StockUpdatedEventHandler> _logger;
    private readonly IEventPublisher _eventPublisher;
    private readonly ISearchEngineService _searchEngineService;

    public StockUpdatedEventHandler(
        ILogger<StockUpdatedEventHandler> logger,
        IEventPublisher eventPublisher,
        ISearchEngineService searchEngineService)
    {
        _logger = logger;
        _eventPublisher = eventPublisher;
        _searchEngineService = searchEngineService;
    }

    public async Task Handle(StockUpdatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling StockUpdatedEvent for product: {ProductId} - {ProductSku}. Stock changed from {OldQuantity} to {NewQuantity}",
            notification.ProductId, notification.ProductSku, notification.OldQuantity, notification.NewQuantity);

        try
        {
            // Publish integration event for other bounded contexts
            var integrationEvent = new StockUpdatedIntegrationEvent(
                notification.ProductId,
                notification.ProductSku,
                notification.OldQuantity,
                notification.NewQuantity,
                notification.TransactionType,
                notification.OccurredOn);

            await _eventPublisher.PublishIntegrationEventAsync(integrationEvent, cancellationToken);

            // Update search index with new stock information
            await _searchEngineService.IndexProductAsync(new
            {
                ProductId = notification.ProductId,
                Sku = notification.ProductSku,
                StockQuantity = notification.NewQuantity,
                IsInStock = notification.NewQuantity > 0,
                LastStockUpdate = notification.OccurredOn
            }, cancellationToken);

            _logger.LogInformation("StockUpdatedEvent handled successfully for product: {ProductId}", notification.ProductId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling StockUpdatedEvent for product: {ProductId}", notification.ProductId);
        }
    }
}

// Integration Events
public class StockUpdatedIntegrationEvent : IIntegrationEvent
{
    public Guid Id { get; }
    public DateTime OccurredOn { get; }
    public string EventType => nameof(StockUpdatedIntegrationEvent);
    public int Version => 1;

    public Guid ProductId { get; }
    public string ProductSku { get; }
    public int OldQuantity { get; }
    public int NewQuantity { get; }
    public TransactionType TransactionType { get; }

    public StockUpdatedIntegrationEvent(
        Guid productId,
        string productSku,
        int oldQuantity,
        int newQuantity,
        TransactionType transactionType,
        DateTime occurredOn)
    {
        Id = Guid.NewGuid();
        ProductId = productId;
        ProductSku = productSku;
        OldQuantity = oldQuantity;
        NewQuantity = newQuantity;
        TransactionType = transactionType;
        OccurredOn = occurredOn;
    }
}