using TechMart.SharedKernel.Base;

namespace TechMart.Product.Domain.Product.Events;

public class ProductBrandChangedEvent : BaseDomainEvent
{
    public Guid ProductId { get; }
    public Guid OldBrandId { get; }
    public Guid NewBrandId { get; }

    public ProductBrandChangedEvent(Guid productId, Guid oldBrandId, Guid newBrandId)
    {
        ProductId = productId;
        OldBrandId = oldBrandId;
        NewBrandId = newBrandId;
    }
}