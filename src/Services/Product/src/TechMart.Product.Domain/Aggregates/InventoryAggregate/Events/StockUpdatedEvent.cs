using TechMart.Product.Domain.Enums;
using TechMart.SharedKernel.Base;

namespace TechMart.Product.Domain.Aggregates.InventoryAggregate.Events;

/// <summary>
/// Domain event raised when stock is updated.
/// </summary>
public class StockUpdatedEvent : BaseDomainEvent
{
    public Guid ProductId { get; }
    public string ProductSku { get; }
    public int OldQuantity { get; }
    public int NewQuantity { get; }
    public TransactionType TransactionType { get; }

    public StockUpdatedEvent(Guid productId, string productSku, int oldQuantity, int newQuantity, TransactionType transactionType)
    {
        ProductId = productId;
        ProductSku = productSku;
        OldQuantity = oldQuantity;
        NewQuantity = newQuantity;
        TransactionType = transactionType;
    }
}