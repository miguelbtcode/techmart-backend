using TechMart.SharedKernel.Base;

namespace TechMart.Product.Domain.Aggregates.ProductAggregate.Events;

public class ProductDeletedEvent : BaseDomainEvent
{
    public Guid ProductId { get; }
    public string Sku { get; }
    public string Name { get; }
    public DateTime DeletedAt { get; }

    public ProductDeletedEvent(Guid productId, string sku, string name)
    {
        ProductId = productId;
        Sku = sku;
        Name = name;
        DeletedAt = DateTime.UtcNow;
    }
}