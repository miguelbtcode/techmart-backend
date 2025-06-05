using TechMart.Product.Domain.Exceptions;
using TechMart.SharedKernel.Base;

namespace TechMart.Product.Domain.Product.ValueObjects;

public class Price : BaseValueObject
{
    public decimal Amount { get; }
    public string Currency { get; }

    public Price(decimal amount, string currency = "USD")
    {
        if (amount < 0)
            throw new InvalidPriceException("Price amount cannot be negative");

        if (string.IsNullOrWhiteSpace(currency))
            throw new ArgumentException("Currency cannot be empty", nameof(currency));

        Amount = Math.Round(amount, 2);
        Currency = currency.ToUpperInvariant();
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }

    public Price Add(Price other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException("Cannot add prices with different currencies");

        return new Price(Amount + other.Amount, Currency);
    }

    public Price Subtract(Price other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException("Cannot subtract prices with different currencies");

        return new Price(Math.Max(0, Amount - other.Amount), Currency);
    }

    public Price Multiply(decimal factor)
    {
        if (factor < 0)
            throw new ArgumentException("Factor cannot be negative", nameof(factor));

        return new Price(Amount * factor, Currency);
    }

    public Price ApplyDiscount(decimal percentage)
    {
        if (percentage < 0 || percentage > 100)
            throw new ArgumentException("Discount percentage must be between 0 and 100", nameof(percentage));

        var discountAmount = Amount * (percentage / 100);
        return new Price(Amount - discountAmount, Currency);
    }

    public bool IsGreaterThan(Price other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException("Cannot compare prices with different currencies");

        return Amount > other.Amount;
    }

    public bool IsLessThan(Price other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException("Cannot compare prices with different currencies");

        return Amount < other.Amount;
    }

    public override string ToString() => $"{Amount:C} {Currency}";

    public string ToDisplayString() => $"{Amount:F2} {Currency}";
}
