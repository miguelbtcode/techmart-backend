using TechMart.SharedKernel.Base;

namespace TechMart.Product.Domain.Aggregates.ProductAggregate.Events;

public class ProductCategoryChangedEvent : BaseDomainEvent
{
    public Guid ProductId { get; }
    public Guid OldCategoryId { get; }
    public Guid NewCategoryId { get; }

    public ProductCategoryChangedEvent(Guid productId, Guid oldCategoryId, Guid newCategoryId)
    {
        ProductId = productId;
        OldCategoryId = oldCategoryId;
        NewCategoryId = newCategoryId;
    }
}