using TechMart.SharedKernel.Common;

namespace TechMart.Product.Domain.Product.Errors;

public static class ProductErrors
{
    // Price errors
    public static Error InvalidPrice(decimal price) => 
        Error.Validation("Product.InvalidPrice", $"Price cannot be negative: {price}");
    
    public static Error ZeroPrice() => 
        Error.Validation("Product.ZeroPrice", "Price cannot be zero");
    
    public static Error PriceTooHigh(decimal price, decimal maxPrice) => 
        Error.Validation("Product.PriceTooHigh", $"Price {price} exceeds maximum allowed price of {maxPrice}");

    // SKU errors
    public static Error InvalidSku(string sku) => 
        Error.Validation("Product.InvalidSku", $"SKU '{sku}' format is invalid. Must be 3-20 alphanumeric characters");
    
    public static Error EmptySku() => 
        Error.Validation("Product.EmptySku", "SKU cannot be empty");
    
    public static Error DuplicateSku(string sku) => 
        Error.Conflict("Product.DuplicateSku", $"SKU '{sku}' already exists");

    // Stock errors
    public static Error NegativeStock(int quantity) => 
        Error.Validation("Product.NegativeStock", $"Stock quantity cannot be negative: {quantity}");
    
    public static Error InsufficientStock(int requested, int available) => 
        Error.Conflict("Product.InsufficientStock", $"Insufficient stock. Requested: {requested}, Available: {available}");

    // Image errors
    public static Error InvalidImageUrl(string url) => 
        Error.Validation("Product.InvalidImageUrl", $"Invalid image URL: {url}");
    
    public static Error UnsupportedImageFormat(string format) => 
        Error.Validation("Product.UnsupportedImageFormat", $"Unsupported image format: {format}");

    // Category errors
    public static Error CircularCategoryReference(Guid categoryId, Guid parentId) => 
        Error.Validation("Product.CircularCategoryReference", $"Category {categoryId} cannot be child of {parentId} - circular reference");

    // General product errors
    public static Error ProductNotFound(Guid productId) => 
        Error.NotFound("Product.NotFound", $"Product with ID '{productId}' not found");
    
    public static Error CannotActivateProduct(string reason) => 
        Error.Validation("Product.CannotActivate", $"Product cannot be activated: {reason}");
}