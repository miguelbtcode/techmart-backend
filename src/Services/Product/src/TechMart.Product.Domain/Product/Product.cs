using TechMart.Product.Domain.Aggregates.ProductAggregate.Events;
using TechMart.Product.Domain.Exceptions;
using TechMart.Product.Domain.Product.Entities;
using TechMart.Product.Domain.Product.Enums;
using TechMart.Product.Domain.Product.Events;
using TechMart.Product.Domain.Product.ValueObjects;
using TechMart.SharedKernel.Base;

namespace TechMart.Product.Domain.Product;

/// <summary>
/// Product aggregate root that represents a product in the system.
/// Follows DDD principles and encapsulates business logic related to products.
/// </summary>
public class Product : BaseAggregateRoot<Guid>
{
    private readonly List<ProductImage> _images = new();
    private readonly List<ProductVariant> _variants = new();
    private readonly List<ProductAttribute> _attributes = new();
    private readonly List<ProductReview> _reviews = new();

    public ProductSku Sku { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string? ShortDescription { get; private set; }
    public Price Price { get; private set; }
    public Price? CompareAtPrice { get; private set; }
    public Weight? Weight { get; private set; }
    public Dimensions? Dimensions { get; private set; }
    public ProductStatus Status { get; private set; }
    public ProductType Type { get; private set; }
    public bool IsDigital { get; private set; }
    public bool RequiresShipping { get; private set; }
    public bool IsActive { get; private set; }
    public bool IsFeatured { get; private set; }
    public int SortOrder { get; private set; }
    public string? SeoTitle { get; private set; }
    public string? SeoDescription { get; private set; }
    public string? Tags { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public string? CreatedBy { get; private set; }
    public string? UpdatedBy { get; private set; }

    // Navigation properties
    public Guid BrandId { get; private set; }
    public Guid CategoryId { get; private set; }

    // Collections
    public IReadOnlyList<ProductImage> Images => _images.AsReadOnly();
    public IReadOnlyList<ProductVariant> Variants => _variants.AsReadOnly();
    public IReadOnlyList<ProductAttribute> Attributes => _attributes.AsReadOnly();
    public IReadOnlyList<ProductReview> Reviews => _reviews.AsReadOnly();

    // Calculated properties
    public decimal AverageRating => _reviews.Any() ? (decimal)_reviews.Average(r => r.Rating) : 0;
    public int ReviewCount => _reviews.Count;
    public bool IsOnSale => CompareAtPrice != null && CompareAtPrice.Amount > Price.Amount;
    public decimal DiscountPercentage => IsOnSale ? 
        Math.Round((1 - (Price.Amount / CompareAtPrice!.Amount)) * 100, 2) : 0;

    // Private constructor for EF Core
    private Product() { }

    public Product(
        ProductSku sku,
        string name,
        string description,
        Price price,
        Guid brandId,
        Guid categoryId,
        ProductType type = ProductType.Physical,
        string? createdBy = null)
    {
        Id = Guid.NewGuid();
        Sku = sku ?? throw new ArgumentNullException(nameof(sku));
        Name = Guard.NotNullOrWhiteSpace(name, nameof(name));
        Description = Guard.NotNullOrWhiteSpace(description, nameof(description));
        Price = price ?? throw new ArgumentNullException(nameof(price));
        BrandId = Guard.NotDefault(brandId, nameof(brandId));
        CategoryId = Guard.NotDefault(categoryId, nameof(categoryId));
        Type = type;
        Status = ProductStatus.Draft;
        IsActive = false;
        IsFeatured = false;
        IsDigital = type == ProductType.Digital;
        RequiresShipping = type == ProductType.Physical;
        SortOrder = 0;
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;

        // Raise domain event
        RaiseDomainEvent(new ProductCreatedEvent(Id, Sku.Value, Name, Price.Amount, BrandId, CategoryId));
    }

    // Business methods

    /// <summary>
    /// Updates basic product information.
    /// </summary>
    public void UpdateBasicInfo(
        string name,
        string description,
        string? shortDescription = null,
        string? seoTitle = null,
        string? seoDescription = null,
        string? tags = null,
        string? updatedBy = null)
    {
        Guard.NotNullOrWhiteSpace(name, nameof(name));
        Guard.NotNullOrWhiteSpace(description, nameof(description));

        var oldName = Name;
        var oldDescription = Description;

        Name = name;
        Description = description;
        ShortDescription = shortDescription;
        SeoTitle = seoTitle;
        SeoDescription = seoDescription;
        Tags = tags;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;

        // Raise event if significant changes occurred
        if (oldName != Name || oldDescription != Description)
        {
            RaiseDomainEvent(new ProductUpdatedEvent(Id, Name, Description));
        }
    }

    /// <summary>
    /// Updates product pricing.
    /// </summary>
    public void UpdatePricing(Price price, Price? compareAtPrice = null, string? updatedBy = null)
    {
        var oldPrice = Price;

        Price = price ?? throw new ArgumentNullException(nameof(price));
        CompareAtPrice = compareAtPrice;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;

        // Raise event if price changed
        if (oldPrice.Amount != Price.Amount)
        {
            RaiseDomainEvent(new ProductPriceChangedEvent(Id, oldPrice.Amount, Price.Amount));
        }
    }

    /// <summary>
    /// Updates product physical properties.
    /// </summary>
    public void UpdatePhysicalProperties(Weight? weight, Dimensions? dimensions, string? updatedBy = null)
    {
        if (IsDigital && (weight != null || dimensions != null))
        {
            throw new ProductDomainException("Digital products cannot have physical properties");
        }

        Weight = weight;
        Dimensions = dimensions;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    /// <summary>
    /// Changes the product status.
    /// </summary>
    public void ChangeStatus(ProductStatus newStatus, string? updatedBy = null)
    {
        if (!CanChangeStatusTo(newStatus))
        {
            throw new ProductDomainException($"Cannot change status from {Status} to {newStatus}");
        }

        var oldStatus = Status;
        Status = newStatus;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;

        // Update IsActive based on status
        IsActive = Status == ProductStatus.Active;

        RaiseDomainEvent(new ProductStatusChangedEvent(Id, oldStatus, newStatus));
    }

    /// <summary>
    /// Activates the product.
    /// </summary>
    public void Activate(string? updatedBy = null)
    {
        if (!CanActivate())
        {
            throw new ProductDomainException("Product cannot be activated. Ensure it has required information.");
        }

        ChangeStatus(ProductStatus.Active, updatedBy);
    }

    /// <summary>
    /// Deactivates the product.
    /// </summary>
    public void Deactivate(string? updatedBy = null)
    {
        ChangeStatus(ProductStatus.Inactive, updatedBy);
    }

    /// <summary>
    /// Archives the product.
    /// </summary>
    public void Archive(string? updatedBy = null)
    {
        ChangeStatus(ProductStatus.Archived, updatedBy);
    }

    /// <summary>
    /// Sets the product as featured.
    /// </summary>
    public void SetFeatured(bool featured, string? updatedBy = null)
    {
        IsFeatured = featured;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    /// <summary>
    /// Updates the sort order.
    /// </summary>
    public void UpdateSortOrder(int sortOrder, string? updatedBy = null)
    {
        SortOrder = Guard.NotNegative(sortOrder, nameof(sortOrder));
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    /// <summary>
    /// Changes the product category.
    /// </summary>
    public void ChangeCategory(Guid categoryId, string? updatedBy = null)
    {
        var oldCategoryId = CategoryId;
        CategoryId = Guard.NotDefault(categoryId, nameof(categoryId));
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;

        RaiseDomainEvent(new ProductCategoryChangedEvent(Id, oldCategoryId, categoryId));
    }

    /// <summary>
    /// Changes the product brand.
    /// </summary>
    public void ChangeBrand(Guid brandId, string? updatedBy = null)
    {
        var oldBrandId = BrandId;
        BrandId = Guard.NotDefault(brandId, nameof(brandId));
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;

        RaiseDomainEvent(new ProductBrandChangedEvent(Id, oldBrandId, brandId));
    }

    // Image management

    /// <summary>
    /// Adds an image to the product.
    /// </summary>
    public void AddImage(ImageUrl imageUrl, string? altText = null, bool isPrimary = false)
    {
        if (isPrimary)
        {
            // Remove primary flag from other images
            foreach (var img in _images.Where(i => i.IsPrimary))
            {
                img.SetAsPrimary(false);
            }
        }

        var image = new ProductImage(Id, imageUrl, altText, isPrimary, _images.Count);
        _images.Add(image);
    }

    /// <summary>
    /// Removes an image from the product.
    /// </summary>
    public void RemoveImage(Guid imageId)
    {
        var image = _images.FirstOrDefault(i => i.Id == imageId);
        if (image != null)
        {
            _images.Remove(image);
            
            // Reorder remaining images
            for (int i = 0; i < _images.Count; i++)
            {
                _images[i].UpdateSortOrder(i);
            }
        }
    }

    /// <summary>
    /// Sets an image as primary.
    /// </summary>
    public void SetPrimaryImage(Guid imageId)
    {
        var image = _images.FirstOrDefault(i => i.Id == imageId);
        if (image == null)
        {
            throw new ProductDomainException("Image not found");
        }

        // Remove primary flag from other images
        foreach (var img in _images.Where(i => i.IsPrimary && i.Id != imageId))
        {
            img.SetAsPrimary(false);
        }

        image.SetAsPrimary(true);
    }

    // Variant management

    /// <summary>
    /// Adds a variant to the product.
    /// </summary>
    public void AddVariant(ProductSku variantSku, string name, Price price, Dictionary<string, string>? attributes = null)
    {
        if (_variants.Any(v => v.Sku.Value == variantSku.Value))
        {
            throw new ProductDomainException($"Variant with SKU {variantSku.Value} already exists");
        }

        var variant = new ProductVariant(Id, variantSku, name, price, attributes);
        _variants.Add(variant);
    }

    /// <summary>
    /// Removes a variant from the product.
    /// </summary>
    public void RemoveVariant(Guid variantId)
    {
        var variant = _variants.FirstOrDefault(v => v.Id == variantId);
        if (variant != null)
        {
            _variants.Remove(variant);
        }
    }

    // Attribute management

    /// <summary>
    /// Adds or updates a product attribute.
    /// </summary>
    public void SetAttribute(string name, string value)
    {
        Guard.NotNullOrWhiteSpace(name, nameof(name));
        Guard.NotNullOrWhiteSpace(value, nameof(value));

        var attribute = _attributes.FirstOrDefault(a => a.Name == name);
        if (attribute != null)
        {
            attribute.UpdateValue(value);
        }
        else
        {
            _attributes.Add(new ProductAttribute(Id, name, value));
        }
    }

    /// <summary>
    /// Removes a product attribute.
    /// </summary>
    public void RemoveAttribute(string name)
    {
        var attribute = _attributes.FirstOrDefault(a => a.Name == name);
        if (attribute != null)
        {
            _attributes.Remove(attribute);
        }
    }

    // Business logic validation

    private bool CanActivate()
    {
        return !string.IsNullOrWhiteSpace(Name) &&
               !string.IsNullOrWhiteSpace(Description) &&
               Price != null &&
               Price.Amount > 0 &&
               BrandId != Guid.Empty &&
               CategoryId != Guid.Empty;
    }

    private bool CanChangeStatusTo(ProductStatus newStatus)
    {
        return newStatus switch
        {
            ProductStatus.Active => CanActivate(),
            ProductStatus.Inactive => Status != ProductStatus.Archived,
            ProductStatus.Draft => Status != ProductStatus.Archived,
            ProductStatus.Archived => true,
            _ => false
        };
    }

    // Helper methods

    public bool HasVariants() => _variants.Any();
    public bool HasImages() => _images.Any();
    public ProductImage? GetPrimaryImage() => _images.FirstOrDefault(i => i.IsPrimary);
    public string? GetAttributeValue(string name) => _attributes.FirstOrDefault(a => a.Name == name)?.Value;
}

/// <summary>
/// Guard helper methods for Product domain.
/// </summary>
internal static class Guard
{
    public static string NotNullOrWhiteSpace(string value, string paramName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException($"{paramName} cannot be null or empty", paramName);
        return value;
    }

    public static T NotDefault<T>(T value, string paramName) where T : struct
    {
        if (value.Equals(default(T)))
            throw new ArgumentException($"{paramName} cannot be default value", paramName);
        return value;
    }

    public static int NotNegative(int value, string paramName)
    {
        if (value < 0)
            throw new ArgumentException($"{paramName} cannot be negative", paramName);
        return value;
    }
}