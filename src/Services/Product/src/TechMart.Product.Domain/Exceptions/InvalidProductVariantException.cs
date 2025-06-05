namespace TechMart.Product.Domain.Exceptions;

/// <summary>
/// Exception thrown when product variants are invalid.
/// </summary>
public class InvalidProductVariantException : ProductDomainException
{
    public Guid? ProductId { get; }
    public Guid? VariantId { get; }
    public string? VariantSku { get; }

    public InvalidProductVariantException(string message) : base("Product.InvalidVariant", message)
    {
    }

    public InvalidProductVariantException(string message, Guid productId, Guid? variantId = null, string? variantSku = null) 
        : base("Product.InvalidVariant", message)
    {
        ProductId = productId;
        VariantId = variantId;
        VariantSku = variantSku;

        WithDetail("ProductId", productId);
        if (variantId.HasValue)
            WithDetail("VariantId", variantId.Value);
        if (!string.IsNullOrEmpty(variantSku))
            WithDetail("VariantSku", variantSku);
    }
    
    public static InvalidProductVariantException DuplicateVariantSku(Guid productId, string variantSku) =>
        new($"Variant with SKU '{variantSku}' already exists for product {productId}", productId, null, variantSku);
    
    public static InvalidProductVariantException VariantNotFound(Guid productId, Guid variantId) =>
        new($"Variant {variantId} not found for product {productId}", productId, variantId);
    
    public static InvalidProductVariantException InvalidAttributes(Guid productId, string reason) =>
        new($"Invalid variant attributes for product {productId}: {reason}", productId);
}