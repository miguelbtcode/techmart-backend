using System.Text.RegularExpressions;
using TechMart.Product.Domain.Exceptions;
using TechMart.SharedKernel.Base;

namespace TechMart.Product.Domain.Product.ValueObjects;

public class ProductSku : BaseValueObject
{
    private static readonly Regex SkuPattern = new(@"^[A-Z0-9]{3,20}$", RegexOptions.Compiled);

    public string Value { get; }

    public ProductSku(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidSkuException("SKU cannot be empty");

        var normalizedValue = value.Trim().ToUpperInvariant();
        
        if (!SkuPattern.IsMatch(normalizedValue))
            throw new InvalidSkuException("SKU must be 3-20 characters long and contain only letters and numbers");

        Value = normalizedValue;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public static implicit operator string(ProductSku sku) => sku.Value;
    public static implicit operator ProductSku(string value) => new(value);

    public override string ToString() => Value;
}