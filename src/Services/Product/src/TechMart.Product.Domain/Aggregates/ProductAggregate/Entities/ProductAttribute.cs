using TechMart.SharedKernel.Base;

namespace TechMart.Product.Domain.Aggregates.ProductAggregate.Entities;

/// <summary>
/// Represents a custom attribute for a product.
/// Used for flexible product specifications and properties.
/// </summary>
public class ProductAttribute : BaseEntity<Guid>
{
    public Guid ProductId { get; private set; }
    public string Name { get; private set; }
    public string Value { get; private set; }
    public string? DisplayName { get; private set; }
    public string? Unit { get; private set; }
    public int SortOrder { get; private set; }
    public bool IsRequired { get; private set; }
    public bool IsVisible { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // Private constructor for EF Core
    private ProductAttribute() { }

    public ProductAttribute(
        Guid productId,
        string name,
        string value,
        string? displayName = null,
        string? unit = null,
        bool isRequired = false,
        bool isVisible = true)
    {
        Id = Guid.NewGuid();
        ProductId = productId;
        Name = !string.IsNullOrWhiteSpace(name) ? name : throw new ArgumentException("Name cannot be empty", nameof(name));
        Value = value ?? string.Empty;
        DisplayName = displayName;
        Unit = unit;
        IsRequired = isRequired;
        IsVisible = isVisible;
        SortOrder = 0;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateValue(string value)
    {
        Value = value ?? string.Empty;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateDisplayInfo(string? displayName, string? unit = null)
    {
        DisplayName = displayName;
        Unit = unit;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateVisibility(bool isVisible)
    {
        IsVisible = isVisible;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateSortOrder(int sortOrder)
    {
        SortOrder = Math.Max(0, sortOrder);
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetRequired(bool isRequired)
    {
        IsRequired = isRequired;
        UpdatedAt = DateTime.UtcNow;
    }

    public string GetDisplayValue()
    {
        return string.IsNullOrWhiteSpace(Unit) ? Value : $"{Value} {Unit}";
    }

    public string GetDisplayName()
    {
        return string.IsNullOrWhiteSpace(DisplayName) ? Name : DisplayName;
    }
}