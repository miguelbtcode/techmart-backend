using TechMart.SharedKernel.Base;

namespace TechMart.Product.Domain.Aggregates.ProductAggregate.Events;

public class ProductPriceChangedEvent : BaseDomainEvent
{
    public Guid ProductId { get; }
    public decimal OldPrice { get; }
    public decimal NewPrice { get; }
    public decimal PriceChange => NewPrice - OldPrice;
    public decimal PriceChangePercentage => OldPrice > 0 ? (PriceChange / OldPrice) * 100 : 0;

    public ProductPriceChangedEvent(Guid productId, decimal oldPrice, decimal newPrice)
    {
        ProductId = productId;
        OldPrice = oldPrice;
        NewPrice = newPrice;
    }
}