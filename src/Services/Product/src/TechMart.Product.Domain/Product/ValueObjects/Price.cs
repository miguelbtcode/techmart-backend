using TechMart.Product.Domain.Product.Errors;
using TechMart.SharedKernel.Base;
using TechMart.SharedKernel.Common;

namespace TechMart.Product.Domain.Product.ValueObjects;

public class Price : BaseValueObject
{
    public decimal Amount { get; private set; }
    public string Currency { get; private set; }

    private Price(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }

    public static Result<Price> Create(decimal amount, string currency = "USD")
    {
        if (amount < 0)
            return Result.Failure<Price>(ProductErrors.InvalidPrice(amount));

        if (amount == 0)
            return Result.Failure<Price>(ProductErrors.ZeroPrice());

        if (string.IsNullOrWhiteSpace(currency))
            return Result.Failure<Price>(Error.Validation("Price.InvalidCurrency", "Currency cannot be empty"));

        return Result.Success(new Price(Math.Round(amount, 2), currency.ToUpperInvariant()));
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }

    public Result<Price> Add(Price other)
    {
        if (Currency != other.Currency)
            return Result.Failure<Price>(Error.Validation("Price.CurrencyMismatch", "Cannot add prices with different currencies"));

        return Create(Amount + other.Amount, Currency);
    }

    public override string ToString() => $"{Amount:F2} {Currency}";
}