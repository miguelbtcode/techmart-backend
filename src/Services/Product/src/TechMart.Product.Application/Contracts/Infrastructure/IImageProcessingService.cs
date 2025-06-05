namespace TechMart.Product.Application.Contracts.Infrastructure;

/// <summary>
/// Service for image processing operations.
/// </summary>
public interface IImageProcessingService
{
    /// <summary>
    /// Resizes an image to the specified dimensions.
    /// </summary>
    /// <param name="imageStream">The image stream.</param>
    /// <param name="width">The target width.</param>
    /// <param name="height">The target height.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The resized image stream.</returns>
    Task<Stream> ResizeImageAsync(Stream imageStream, int width, int height, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates thumbnails for an image.
    /// </summary>
    /// <param name="imageStream">The image stream.</param>
    /// <param name="sizes">The thumbnail sizes.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Dictionary of size to thumbnail stream.</returns>
    Task<Dictionary<string, Stream>> GenerateThumbnailsAsync(Stream imageStream, int[] sizes, CancellationToken cancellationToken = default);

    /// <summary>
    /// Optimizes an image for web usage.
    /// </summary>
    /// <param name="imageStream">The image stream.</param>
    /// <param name="quality">The compression quality (1-100).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The optimized image stream.</returns>
    Task<Stream> OptimizeImageAsync(Stream imageStream, int quality = 85, CancellationToken cancellationToken = default);
}