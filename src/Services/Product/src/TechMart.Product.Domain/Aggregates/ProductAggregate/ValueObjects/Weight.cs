using TechMart.SharedKernel.Base;

namespace TechMart.Product.Domain.Aggregates.ProductAggregate.ValueObjects;

public class Weight : BaseValueObject
{
    public decimal Value { get; }
    public string Unit { get; }

    public Weight(decimal value, string unit = "kg")
    {
        if (value < 0)
            throw new ArgumentException("Weight cannot be negative", nameof(value));

        if (string.IsNullOrWhiteSpace(unit))
            throw new ArgumentException("Unit cannot be empty", nameof(unit));

        Value = Math.Round(value, 3);
        Unit = unit.ToLowerInvariant();
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
        yield return Unit;
    }

    public Weight ConvertTo(string targetUnit)
    {
        var valueInGrams = Unit switch
        {
            "g" => Value,
            "kg" => Value * 1000,
            "lb" => Value * 453.592m,
            "oz" => Value * 28.3495m,
            _ => throw new NotSupportedException($"Unit {Unit} is not supported")
        };

        var convertedValue = targetUnit.ToLowerInvariant() switch
        {
            "g" => valueInGrams,
            "kg" => valueInGrams / 1000,
            "lb" => valueInGrams / 453.592m,
            "oz" => valueInGrams / 28.3495m,
            _ => throw new NotSupportedException($"Target unit {targetUnit} is not supported")
        };

        return new Weight(convertedValue, targetUnit);
    }

    public bool IsGreaterThan(Weight other)
    {
        var otherInSameUnit = other.ConvertTo(Unit);
        return Value > otherInSameUnit.Value;
    }

    public Weight Add(Weight other)
    {
        var otherInSameUnit = other.ConvertTo(Unit);
        return new Weight(Value + otherInSameUnit.Value, Unit);
    }

    public override string ToString() => $"{Value:F3} {Unit}";
}