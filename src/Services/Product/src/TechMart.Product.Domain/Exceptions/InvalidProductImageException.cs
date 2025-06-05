namespace TechMart.Product.Domain.Exceptions;

public class InvalidProductImageException : ProductDomainException
{
    public Guid? ProductId { get; }
    public Guid? ImageId { get; }
    public string? ImageUrl { get; }

    public InvalidProductImageException(string message) : base("Product.InvalidImage", message)
    {
    }

    public InvalidProductImageException(string message, Guid productId, Guid? imageId = null, string? imageUrl = null) 
        : base("Product.InvalidImage", message)
    {
        ProductId = productId;
        ImageId = imageId;
        ImageUrl = imageUrl;

        WithDetail("ProductId", productId);
        if (imageId.HasValue)
            WithDetail("ImageId", imageId.Value);
        if (!string.IsNullOrEmpty(imageUrl))
            WithDetail("ImageUrl", imageUrl);
    }
    
    public static InvalidProductImageException InvalidUrl(string imageUrl) =>
        new($"Invalid image URL: {imageUrl}", Guid.Empty, null, imageUrl);
    
    public static InvalidProductImageException UnsupportedFormat(string imageUrl, string format) =>
        new($"Unsupported image format '{format}' for URL: {imageUrl}", Guid.Empty, null, imageUrl);
    
    public static InvalidProductImageException MaxImagesExceeded(Guid productId, int maxImages) =>
        new($"Product {productId} cannot have more than {maxImages} images", productId);
    
    public static InvalidProductImageException ImageNotFound(Guid productId, Guid imageId) =>
        new($"Image {imageId} not found for product {productId}", productId, imageId);
}