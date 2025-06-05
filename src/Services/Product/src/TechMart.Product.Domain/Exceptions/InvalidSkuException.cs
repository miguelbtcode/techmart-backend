namespace TechMart.Product.Domain.Exceptions;

/// <summary>
/// Exception thrown when a product SKU is invalid.
/// </summary>
public class InvalidSkuException : ProductDomainException
{
    public string? AttemptedSku { get; }

    public InvalidSkuException(string message) : base("Product.InvalidSku", message)
    {
    }

    public InvalidSkuException(string message, string attemptedSku) : base("Product.InvalidSku", message)
    {
        AttemptedSku = attemptedSku;
        WithDetail("AttemptedSku", attemptedSku);
    }
    
    public static InvalidSkuException EmptySku() =>
        new("SKU cannot be empty or null");
    
    public static InvalidSkuException InvalidFormat(string sku) =>
        new($"SKU '{sku}' does not match required format (3-20 alphanumeric characters)", sku);
    
    public static InvalidSkuException DuplicateSku(string sku) =>
        new($"SKU '{sku}' already exists", sku);
    
    public static InvalidSkuException InvalidLength(string sku, int minLength, int maxLength) =>
        new($"SKU '{sku}' must be between {minLength} and {maxLength} characters long", sku);
}