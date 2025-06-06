using System.Text.RegularExpressions;
using TechMart.Product.Domain.Product.Errors;
using TechMart.SharedKernel.Base;
using TechMart.SharedKernel.Common;

namespace TechMart.Product.Domain.Product.ValueObjects;

public class ProductSku : BaseValueObject
{
    private static readonly Regex SkuPattern = new(@"^[A-Z0-9]{3,20}$", RegexOptions.Compiled);

    public string Value { get; private set; }

    private ProductSku(string value)
    {
        Value = value;
    }

    public static Result<ProductSku> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Failure<ProductSku>(ProductErrors.EmptySku());

        var normalizedValue = value.Trim().ToUpperInvariant();
        
        if (!SkuPattern.IsMatch(normalizedValue))
            return Result.Failure<ProductSku>(ProductErrors.InvalidSku(value));

        return Result.Success(new ProductSku(normalizedValue));
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public static implicit operator string(ProductSku sku) => sku.Value;
    public override string ToString() => Value;
}