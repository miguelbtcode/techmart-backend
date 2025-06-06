using TechMart.Product.Domain.Product.Enums;

namespace TechMart.Product.Application.Features.Products.Vms;

public record ProductVm
{
    public Guid Id { get; set; }
    public string Sku { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ShortDescription { get; set; }
    public decimal Price { get; set; }
    public string Currency { get; set; } = "USD";
    public decimal? CompareAtPrice { get; set; }
    public ProductStatus Status { get; set; }
    public ProductType Type { get; set; }
    public bool IsActive { get; set; }
    public bool IsFeatured { get; set; }
    public bool IsDigital { get; set; }
    public bool RequiresShipping { get; set; }
    public decimal? Weight { get; set; }
    public string? WeightUnit { get; set; }
    public string? Tags { get; set; }
    public Guid BrandId { get; set; }
    public string? BrandName { get; set; }
    public Guid CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Collections
    public List<ProductImageVm> Images { get; set; } = new();
    public List<ProductVariantVm> Variants { get; set; } = new();
    public List<ProductAttributeVm> Attributes { get; set; } = new();

    // Calculated properties
    public decimal AverageRating { get; set; }
    public int ReviewCount { get; set; }
    public bool IsOnSale { get; set; }
    public decimal DiscountPercentage { get; set; }
}