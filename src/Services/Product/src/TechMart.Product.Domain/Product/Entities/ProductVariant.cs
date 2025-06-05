using TechMart.Product.Domain.Product.ValueObjects;
using TechMart.SharedKernel.Base;

namespace TechMart.Product.Domain.Product.Entities;

/// <summary>
/// Represents a product variant within the product aggregate.
/// Used for products that have different options (size, color, etc.).
/// </summary>
public class ProductVariant : BaseEntity<Guid>
{
    private readonly Dictionary<string, string> _attributes = new();

    public Guid ProductId { get; private set; }
    public ProductSku Sku { get; private set; }
    public string Name { get; private set; }
    public Price Price { get; private set; }
    public Price? CompareAtPrice { get; private set; }
    public bool IsActive { get; private set; }
    public int SortOrder { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public IReadOnlyDictionary<string, string> Attributes => _attributes.AsReadOnly();

    // Calculated properties
    public bool IsOnSale => CompareAtPrice != null && CompareAtPrice.Amount > Price.Amount;
    public decimal DiscountPercentage => IsOnSale ? 
        Math.Round((1 - (Price.Amount / CompareAtPrice!.Amount)) * 100, 2) : 0;

    // Private constructor for EF Core
    private ProductVariant() { }

    public ProductVariant(
        Guid productId,
        ProductSku sku,
        string name,
        Price price,
        Dictionary<string, string>? attributes = null,
        Price? compareAtPrice = null)
    {
        Id = Guid.NewGuid();
        ProductId = productId;
        Sku = sku ?? throw new ArgumentNullException(nameof(sku));
        Name = !string.IsNullOrWhiteSpace(name) ? name : throw new ArgumentException("Name cannot be empty", nameof(name));
        Price = price ?? throw new ArgumentNullException(nameof(price));
        CompareAtPrice = compareAtPrice;
        IsActive = true;
        SortOrder = 0;
        CreatedAt = DateTime.UtcNow;

        if (attributes != null)
        {
            foreach (var attr in attributes)
            {
                _attributes[attr.Key] = attr.Value;
            }
        }
    }

    public void UpdateBasicInfo(string name, Price price, Price? compareAtPrice = null)
    {
        Name = !string.IsNullOrWhiteSpace(name) ? name : throw new ArgumentException("Name cannot be empty", nameof(name));
        Price = price ?? throw new ArgumentNullException(nameof(price));
        CompareAtPrice = compareAtPrice;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetAttribute(string key, string value)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Attribute key cannot be empty", nameof(key));
        
        _attributes[key] = value ?? string.Empty;
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveAttribute(string key)
    {
        _attributes.Remove(key);
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateSortOrder(int sortOrder)
    {
        SortOrder = Math.Max(0, sortOrder);
        UpdatedAt = DateTime.UtcNow;
    }

    public string? GetAttributeValue(string key) => _attributes.TryGetValue(key, out var value) ? value : null;
}