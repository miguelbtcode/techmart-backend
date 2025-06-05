using TechMart.SharedKernel.Base;

namespace TechMart.Product.Domain.Aggregates.ProductAggregate.ValueObjects;

public class Dimensions : BaseValueObject
{
    public decimal Length { get; }
    public decimal Width { get; }
    public decimal Height { get; }
    public string Unit { get; }

    public Dimensions(decimal length, decimal width, decimal height, string unit = "cm")
    {
        if (length < 0 || width < 0 || height < 0)
            throw new ArgumentException("Dimensions cannot be negative");

        if (string.IsNullOrWhiteSpace(unit))
            throw new ArgumentException("Unit cannot be empty", nameof(unit));

        Length = Math.Round(length, 2);
        Width = Math.Round(width, 2);
        Height = Math.Round(height, 2);
        Unit = unit.ToLowerInvariant();
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Length;
        yield return Width;
        yield return Height;
        yield return Unit;
    }

    public decimal Volume => Length * Width * Height;
    
    public decimal SurfaceArea => 2 * (Length * Width + Width * Height + Height * Length);

    public Dimensions ConvertTo(string targetUnit)
    {
        var conversionFactor = GetConversionFactor(Unit, targetUnit);
        
        return new Dimensions(
            Length * conversionFactor,
            Width * conversionFactor,
            Height * conversionFactor,
            targetUnit
        );
    }

    private static decimal GetConversionFactor(string fromUnit, string toUnit)
    {
        // Convert to meters first, then to target unit
        var fromToMeters = fromUnit switch
        {
            "mm" => 0.001m,
            "cm" => 0.01m,
            "m" => 1m,
            "in" => 0.0254m,
            "ft" => 0.3048m,
            _ => throw new NotSupportedException($"Unit {fromUnit} is not supported")
        };

        var metersToTarget = toUnit switch
        {
            "mm" => 1000m,
            "cm" => 100m,
            "m" => 1m,
            "in" => 39.3701m,
            "ft" => 3.28084m,
            _ => throw new NotSupportedException($"Unit {toUnit} is not supported")
        };

        return fromToMeters * metersToTarget;
    }

    public override string ToString() => $"{Length} x {Width} x {Height} {Unit}";
}