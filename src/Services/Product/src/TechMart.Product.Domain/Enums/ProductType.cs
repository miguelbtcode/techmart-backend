using TechMart.SharedKernel.Base;

namespace TechMart.Product.Domain.Enums;

/// <summary>
/// Represents the type of a product.
/// </summary>
public enum ProductType
{
    /// <summary>
    /// Physical product that requires shipping.
    /// </summary>
    Physical = 1,

    /// <summary>
    /// Digital product that doesn't require shipping.
    /// </summary>
    Digital = 2,

    /// <summary>
    /// Service that is provided to the customer.
    /// </summary>
    Service = 3,

    /// <summary>
    /// Bundle of multiple products.
    /// </summary>
    Bundle = 4,

    /// <summary>
    /// Subscription-based product.
    /// </summary>
    Subscription = 5
}

/// <summary>
/// Type-safe enumeration for ProductType with additional functionality.
/// </summary>
public class ProductTypeEnumeration : Enumeration
{
    public static readonly ProductTypeEnumeration Physical = new(1, nameof(Physical), "Physical Product", true, true);
    public static readonly ProductTypeEnumeration Digital = new(2, nameof(Digital), "Digital Product", false, false);
    public static readonly ProductTypeEnumeration Service = new(3, nameof(Service), "Service", false, false);
    public static readonly ProductTypeEnumeration Bundle = new(4, nameof(Bundle), "Product Bundle", true, true);
    public static readonly ProductTypeEnumeration Subscription = new(5, nameof(Subscription), "Subscription", false, false);

    public string DisplayName { get; }
    public bool RequiresShipping { get; }
    public bool HasPhysicalAttributes { get; }

    private ProductTypeEnumeration(int id, string name, string displayName, bool requiresShipping, bool hasPhysicalAttributes) 
        : base(id, name)
    {
        DisplayName = displayName;
        RequiresShipping = requiresShipping;
        HasPhysicalAttributes = hasPhysicalAttributes;
    }

    /// <summary>
    /// Gets a value indicating whether the product type is digital.
    /// </summary>
    public bool IsDigital => this == Digital;

    /// <summary>
    /// Gets a value indicating whether the product type supports inventory tracking.
    /// </summary>
    public bool SupportsInventory => this == Physical || this == Bundle;

    /// <summary>
    /// Gets all product types that require shipping.
    /// </summary>
    public static IEnumerable<ProductTypeEnumeration> GetShippableTypes() =>
        GetAll<ProductTypeEnumeration>().Where(t => t.RequiresShipping);
}