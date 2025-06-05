using TechMart.SharedKernel.Base;

namespace TechMart.Product.Domain.Product.Events;

public class ProductCreatedEvent : BaseDomainEvent
{
    public Guid ProductId { get; }
    public string Sku { get; }
    public string Name { get; }
    public decimal Price { get; }
    public Guid BrandId { get; }
    public Guid CategoryId { get; }

    public ProductCreatedEvent(
        Guid productId, 
        string sku, 
        string name, 
        decimal price, 
        Guid brandId, 
        Guid categoryId)
    {
        ProductId = productId;
        Sku = sku;
        Name = name;
        Price = price;
        BrandId = brandId;
        CategoryId = categoryId;
    }
}