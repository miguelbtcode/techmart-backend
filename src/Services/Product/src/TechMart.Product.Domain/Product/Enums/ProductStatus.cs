using TechMart.SharedKernel.Base;

namespace TechMart.Product.Domain.Product.Enums;

public enum ProductStatus
{
    /// <summary>
    /// Product is in draft state and not visible to customers.
    /// </summary>
    Draft = 1,

    /// <summary>
    /// Product is active and available for purchase.
    /// </summary>
    Active = 2,

    /// <summary>
    /// Product is temporarily inactive but not deleted.
    /// </summary>
    Inactive = 3,

    /// <summary>
    /// Product is archived and no longer available.
    /// </summary>
    Archived = 4
}

/// <summary>
/// Type-safe enumeration for ProductStatus with additional functionality.
/// </summary>
public class ProductStatusEnumeration : Enumeration
{
    public static readonly ProductStatusEnumeration Draft = new(1, nameof(Draft), "Draft", "Product is being prepared");
    public static readonly ProductStatusEnumeration Active = new(2, nameof(Active), "Active", "Product is available for purchase");
    public static readonly ProductStatusEnumeration Inactive = new(3, nameof(Inactive), "Inactive", "Product is temporarily unavailable");
    public static readonly ProductStatusEnumeration Archived = new(4, nameof(Archived), "Archived", "Product is no longer available");

    public string DisplayName { get; }
    public string Description { get; }

    private ProductStatusEnumeration(int id, string name, string displayName, string description) : base(id, name)
    {
        DisplayName = displayName;
        Description = description;
    }

    /// <summary>
    /// Gets a value indicating whether the product is visible to customers.
    /// </summary>
    public bool IsVisibleToCustomers => this == Active;

    /// <summary>
    /// Gets a value indicating whether the product can be purchased.
    /// </summary>
    public bool IsPurchasable => this == Active;

    /// <summary>
    /// Gets a value indicating whether the product can be modified.
    /// </summary>
    public bool CanBeModified => this != Archived;

    /// <summary>
    /// Gets all statuses that allow the product to be visible.
    /// </summary>
    public static IEnumerable<ProductStatusEnumeration> GetVisibleStatuses() =>
        new[] { Active };

    /// <summary>
    /// Gets all statuses that allow modification.
    /// </summary>
    public static IEnumerable<ProductStatusEnumeration> GetModifiableStatuses() =>
        new[] { Draft, Active, Inactive };
}