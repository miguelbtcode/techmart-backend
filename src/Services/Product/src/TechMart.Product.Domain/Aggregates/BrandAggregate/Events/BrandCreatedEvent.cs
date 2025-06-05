using TechMart.SharedKernel.Base;

namespace TechMart.Product.Domain.Aggregates.BrandAggregate.Events;

public class BrandCreatedEvent : BaseDomainEvent
{
    public Guid BrandId { get; }
    public string Name { get; }

    public BrandCreatedEvent(Guid brandId, string name)
    {
        BrandId = brandId;
        Name = name;
    }
}