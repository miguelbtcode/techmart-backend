using TechMart.SharedKernel.Base;

namespace TechMart.Product.Domain.Aggregates.ProductAggregate.Events;

public class ProductUpdatedEvent : BaseDomainEvent
{
    public Guid ProductId { get; }
    public string Name { get; }
    public string Description { get; }

    public ProductUpdatedEvent(Guid productId, string name, string description)
    {
        ProductId = productId;
        Name = name;
        Description = description;
    }
}
