using TechMart.SharedKernel.Base;

namespace TechMart.Product.Domain.Aggregates.InventoryAggregate.Events;

/// <summary>
/// Domain event raised when stock is low.
/// </summary>
public class LowStockDetectedEvent : BaseDomainEvent
{
    public Guid ProductId { get; }
    public string ProductSku { get; }
    public int CurrentStock { get; }
    public int ReorderLevel { get; }

    public LowStockDetectedEvent(Guid productId, string productSku, int currentStock, int reorderLevel)
    {
        ProductId = productId;
        ProductSku = productSku;
        CurrentStock = currentStock;
        ReorderLevel = reorderLevel;
    }
}