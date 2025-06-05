using TechMart.SharedKernel.Base;
using TechMart.Product.Domain.Aggregates.ProductAggregate.ValueObjects;
using TechMart.Product.Domain.Enums;

namespace TechMart.Product.Domain.Aggregates.ProductAggregate.Entities;

/// <summary>
/// Represents a product image within the product aggregate.
/// </summary>
public class ProductImage : BaseEntity<Guid>
{
    public Guid ProductId { get; private set; }
    public ImageUrl ImageUrl { get; private set; }
    public string? AltText { get; private set; }
    public bool IsPrimary { get; private set; }
    public int SortOrder { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // Private constructor for EF Core
    private ProductImage() { }

    public ProductImage(
        Guid productId, 
        ImageUrl imageUrl, 
        string? altText = null, 
        bool isPrimary = false, 
        int sortOrder = 0)
    {
        Id = Guid.NewGuid();
        ProductId = productId;
        ImageUrl = imageUrl ?? throw new ArgumentNullException(nameof(imageUrl));
        AltText = altText;
        IsPrimary = isPrimary;
        SortOrder = sortOrder;
        CreatedAt = DateTime.UtcNow;
    }

    public void SetAsPrimary(bool isPrimary)
    {
        IsPrimary = isPrimary;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateAltText(string? altText)
    {
        AltText = altText;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateSortOrder(int sortOrder)
    {
        SortOrder = sortOrder;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateImageUrl(ImageUrl imageUrl)
    {
        ImageUrl = imageUrl ?? throw new ArgumentNullException(nameof(imageUrl));
        UpdatedAt = DateTime.UtcNow;
    }
}