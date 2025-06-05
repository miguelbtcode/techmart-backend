using TechMart.SharedKernel.Base;

namespace TechMart.Product.Domain.Enums;

/// <summary>
/// Represents the type of inventory transaction.
/// </summary>
public enum TransactionType
{
    /// <summary>
    /// Stock was added to inventory.
    /// </summary>
    StockIn = 1,

    /// <summary>
    /// Stock was removed from inventory.
    /// </summary>
    StockOut = 2,

    /// <summary>
    /// Stock was reserved for an order.
    /// </summary>
    Reserved = 3,

    /// <summary>
    /// Reserved stock was released.
    /// </summary>
    Released = 4,

    /// <summary>
    /// Stock was adjusted due to audit/count.
    /// </summary>
    Adjustment = 5,

    /// <summary>
    /// Stock was returned.
    /// </summary>
    Return = 6,

    /// <summary>
    /// Stock was transferred between locations.
    /// </summary>
    Transfer = 7,

    /// <summary>
    /// Stock was damaged or lost.
    /// </summary>
    Shrinkage = 8
}

/// <summary>
/// Type-safe enumeration for TransactionType with additional functionality.
/// </summary>
public class TransactionTypeEnumeration : Enumeration
{
    public static readonly TransactionTypeEnumeration StockIn = new(1, nameof(StockIn), "Stock In", true);
    public static readonly TransactionTypeEnumeration StockOut = new(2, nameof(StockOut), "Stock Out", false);
    public static readonly TransactionTypeEnumeration Reserved = new(3, nameof(Reserved), "Reserved", false);
    public static readonly TransactionTypeEnumeration Released = new(4, nameof(Released), "Released", true);
    public static readonly TransactionTypeEnumeration Adjustment = new(5, nameof(Adjustment), "Adjustment", null);
    public static readonly TransactionTypeEnumeration Return = new(6, nameof(Return), "Return", true);
    public static readonly TransactionTypeEnumeration Transfer = new(7, nameof(Transfer), "Transfer", null);
    public static readonly TransactionTypeEnumeration Shrinkage = new(8, nameof(Shrinkage), "Shrinkage", false);

    public string DisplayName { get; }
    public bool? IncreasesStock { get; }

    private TransactionTypeEnumeration(int id, string name, string displayName, bool? increasesStock) : base(id, name)
    {
        DisplayName = displayName;
        IncreasesStock = increasesStock;
    }

    /// <summary>
    /// Gets a value indicating whether the transaction increases stock.
    /// </summary>
    public bool IsPositive => IncreasesStock == true;

    /// <summary>
    /// Gets a value indicating whether the transaction decreases stock.
    /// </summary>
    public bool IsNegative => IncreasesStock == false;

    /// <summary>
    /// Gets a value indicating whether the transaction is neutral (no stock change).
    /// </summary>
    public bool IsNeutral => IncreasesStock == null;
}