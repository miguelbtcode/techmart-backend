using MediatR;
using Microsoft.Extensions.Logging;
using TechMart.Product.Application.Contracts.Infrastructure;
using TechMart.Product.Domain.Inventory.Events;
using TechMart.SharedKernel.Abstractions;

namespace TechMart.Product.Application.Features.Inventory.Events;

/// <summary>
/// Handles LowStockDetectedEvent to send notifications.
/// </summary>
public class LowStockDetectedEventHandler : INotificationHandler<LowStockDetectedEvent>
{
    private readonly ILogger<LowStockDetectedEventHandler> _logger;
    private readonly INotificationService _notificationService;
    private readonly IEventPublisher _eventPublisher;

    public LowStockDetectedEventHandler(
        ILogger<LowStockDetectedEventHandler> logger,
        INotificationService notificationService,
        IEventPublisher eventPublisher)
    {
        _logger = logger;
        _notificationService = notificationService;
        _eventPublisher = eventPublisher;
    }

    public async Task Handle(LowStockDetectedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogWarning("Low stock detected for product: {ProductId} - {ProductSku}. Current stock: {CurrentStock}, Reorder level: {ReorderLevel}",
            notification.ProductId, notification.ProductSku, notification.CurrentStock, notification.ReorderLevel);

        try
        {
            // Send low stock notification
            await _notificationService.SendLowStockAlertAsync(
                notification.ProductId,
                notification.ProductSku, // Using SKU as name for now
                notification.CurrentStock,
                notification.ReorderLevel,
                cancellationToken);

            // Publish integration event
            var integrationEvent = new LowStockDetectedIntegrationEvent(
                notification.ProductId,
                notification.ProductSku,
                notification.CurrentStock,
                notification.ReorderLevel,
                notification.OccurredOn);

            await _eventPublisher.PublishIntegrationEventAsync(integrationEvent, cancellationToken);

            _logger.LogInformation("LowStockDetectedEvent handled successfully for product: {ProductId}", notification.ProductId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling LowStockDetectedEvent for product: {ProductId}", notification.ProductId);
        }
    }
}

public class LowStockDetectedIntegrationEvent : IIntegrationEvent
{
    public Guid Id { get; }
    public DateTime OccurredOn { get; }
    public string EventType => nameof(LowStockDetectedIntegrationEvent);
    public int Version => 1;

    public Guid ProductId { get; }
    public string ProductSku { get; }
    public int CurrentStock { get; }
    public int ReorderLevel { get; }

    public LowStockDetectedIntegrationEvent(
        Guid productId,
        string productSku,
        int currentStock,
        int reorderLevel,
        DateTime occurredOn)
    {
        Id = Guid.NewGuid();
        ProductId = productId;
        ProductSku = productSku;
        CurrentStock = currentStock;
        ReorderLevel = reorderLevel;
        OccurredOn = occurredOn;
    }
}