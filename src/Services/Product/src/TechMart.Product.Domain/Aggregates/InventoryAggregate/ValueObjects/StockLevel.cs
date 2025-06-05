using TechMart.SharedKernel.Base;

namespace TechMart.Product.Domain.Aggregates.InventoryAggregate.ValueObjects;

/// <summary>
/// Stock level value object.
/// </summary>
public class StockLevel : BaseValueObject
{
    public int Current { get; }
    public int Reserved { get; }
    public int Available { get; }
    public int ReorderPoint { get; }

    public StockLevel(int current, int reserved, int reorderPoint)
    {
        Current = Math.Max(0, current);
        Reserved = Math.Max(0, reserved);
        Available = Math.Max(0, Current - Reserved);
        ReorderPoint = Math.Max(0, reorderPoint);
    }

    public bool IsLow => Current <= ReorderPoint;
    public bool IsEmpty => Current <= 0;
    public bool CanFulfill(int quantity) => Available >= quantity;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Current;
        yield return Reserved;
        yield return ReorderPoint;
    }

    public override string ToString() => $"{Available}/{Current} (Reserved: {Reserved})";
}