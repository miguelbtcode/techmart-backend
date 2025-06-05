using TechMart.Product.Domain.Enums;
using TechMart.SharedKernel.Base;

namespace TechMart.Product.Domain.Aggregates.ProductAggregate.Events;

public class ProductStatusChangedEvent : BaseDomainEvent
{
    public Guid ProductId { get; }
    public ProductStatus OldStatus { get; }
    public ProductStatus NewStatus { get; }

    public ProductStatusChangedEvent(Guid productId, ProductStatus oldStatus, ProductStatus newStatus)
    {
        ProductId = productId;
        OldStatus = oldStatus;
        NewStatus = newStatus;
    }
}