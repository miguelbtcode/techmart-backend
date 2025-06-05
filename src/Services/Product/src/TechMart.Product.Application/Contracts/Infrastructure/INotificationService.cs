namespace TechMart.Product.Application.Contracts.Infrastructure;

/// <summary>
/// Service for sending notifications.
/// </summary>
public interface INotificationService
{
    /// <summary>
    /// Sends a low stock alert.
    /// </summary>
    /// <param name="productId">The product ID.</param>
    /// <param name="productName">The product name.</param>
    /// <param name="currentStock">The current stock level.</param>
    /// <param name="reorderLevel">The reorder level.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task SendLowStockAlertAsync(Guid productId, string productName, int currentStock, int reorderLevel, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a product availability notification.
    /// </summary>
    /// <param name="productId">The product ID.</param>
    /// <param name="productName">The product name.</param>
    /// <param name="isAvailable">Whether the product is available.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task SendProductAvailabilityNotificationAsync(Guid productId, string productName, bool isAvailable, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a price change notification.
    /// </summary>
    /// <param name="productId">The product ID.</param>
    /// <param name="productName">The product name.</param>
    /// <param name="oldPrice">The old price.</param>
    /// <param name="newPrice">The new price.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task SendPriceChangeNotificationAsync(Guid productId, string productName, decimal oldPrice, decimal newPrice, CancellationToken cancellationToken = default);
}