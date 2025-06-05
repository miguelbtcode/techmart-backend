namespace TechMart.Product.Domain.Exceptions;

/// <summary>
/// Exception thrown when a product price is invalid.
/// </summary>
public class InvalidPriceException : ProductDomainException
{
    public decimal AttemptedPrice { get; }
    public string? Currency { get; }

    public InvalidPriceException(string message) : base("Product.InvalidPrice", message)
    {
    }

    public InvalidPriceException(decimal attemptedPrice, string? currency = null) 
        : base("Product.InvalidPrice", $"Invalid price: {attemptedPrice} {currency ?? "USD"}")
    {
        AttemptedPrice = attemptedPrice;
        Currency = currency;

        WithDetail("AttemptedPrice", attemptedPrice);
        if (!string.IsNullOrEmpty(currency))
            WithDetail("Currency", currency);
    }

    public InvalidPriceException(string message, decimal attemptedPrice, string? currency = null) 
        : base("Product.InvalidPrice", message)
    {
        AttemptedPrice = attemptedPrice;
        Currency = currency;

        WithDetail("AttemptedPrice", attemptedPrice);
        if (!string.IsNullOrEmpty(currency))
            WithDetail("Currency", currency);
    }
    
    public static InvalidPriceException NegativePrice(decimal price) =>
        new($"Price cannot be negative: {price}", price);
    
    public static InvalidPriceException ZeroPrice() =>
        new("Price cannot be zero", 0);
    
    public static InvalidPriceException PriceTooHigh(decimal price, decimal maxPrice) =>
        new($"Price {price} exceeds maximum allowed price of {maxPrice}", price);
    
    public static InvalidPriceException InvalidCurrency(string currency) =>
        new($"Invalid currency: {currency}");
}