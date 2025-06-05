using TechMart.SharedKernel.Base;

namespace TechMart.Product.Domain.Product.Events;

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