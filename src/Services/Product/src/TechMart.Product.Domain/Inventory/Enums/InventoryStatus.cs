using TechMart.SharedKernel.Base;

namespace TechMart.Product.Domain.Inventory.Enums;

/// <summary>
/// Represents the status of inventory.
/// </summary>
public enum InventoryStatus
{
    InStock = 1,
    LowStock = 2,
    OutOfStock = 3,
    Backorder = 4,
    Discontinued = 5
}

/// <summary>
/// Type-safe enumeration for InventoryStatus with additional functionality.
/// </summary>
public class InventoryStatusEnumeration : Enumeration
{
    public static readonly InventoryStatusEnumeration InStock = new(1, nameof(InStock), "In Stock", "Available for purchase");
    public static readonly InventoryStatusEnumeration LowStock = new(2, nameof(LowStock), "Low Stock", "Limited quantity available");
    public static readonly InventoryStatusEnumeration OutOfStock = new(3, nameof(OutOfStock), "Out of Stock", "Currently unavailable");
    public static readonly InventoryStatusEnumeration Backorder = new(4, nameof(Backorder), "Backorder", "Available for pre-order");
    public static readonly InventoryStatusEnumeration Discontinued = new(5, nameof(Discontinued), "Discontinued", "No longer available");

    public string DisplayName { get; }
    public string Description { get; }

    private InventoryStatusEnumeration(int id, string name, string displayName, string description) : base(id, name)
    {
        DisplayName = displayName;
        Description = description;
    }

    /// <summary>
    /// Gets a value indicating whether the item is available for purchase.
    /// </summary>
    public bool IsAvailable => this == InStock || this == LowStock || this == Backorder;

    /// <summary>
    /// Gets a value indicating whether the item requires attention.
    /// </summary>
    public bool RequiresAttention => this == LowStock || this == OutOfStock;

    /// <summary>
    /// Gets a value indicating whether the item can be restocked.
    /// </summary>
    public bool CanBeRestocked => this != Discontinued;
}
