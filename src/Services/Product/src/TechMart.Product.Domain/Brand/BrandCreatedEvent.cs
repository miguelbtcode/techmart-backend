using TechMart.SharedKernel.Base;

namespace TechMart.Product.Domain.Brand;

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