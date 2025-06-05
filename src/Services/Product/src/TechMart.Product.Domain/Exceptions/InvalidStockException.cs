namespace TechMart.Product.Domain.Exceptions;

public class InvalidStockException : ProductDomainException
{
    public int? AttemptedQuantity { get; }
    public int? CurrentStock { get; }
    public Guid? ProductId { get; }

    public InvalidStockException(string message) : base("Product.InvalidStock", message)
    {
    }

    public InvalidStockException(string message, int attemptedQuantity, int? currentStock = null, Guid? productId = null) 
        : base("Product.InvalidStock", message)
    {
        AttemptedQuantity = attemptedQuantity;
        CurrentStock = currentStock;
        ProductId = productId;

        WithDetail("AttemptedQuantity", attemptedQuantity);
        if (currentStock.HasValue)
            WithDetail("CurrentStock", currentStock.Value);
        if (productId.HasValue)
            WithDetail("ProductId", productId.Value);
    }
    
    public static InvalidStockException NegativeStock(int quantity) =>
        new($"Stock quantity cannot be negative: {quantity}", quantity);
    
    public static InvalidStockException InsufficientStock(int requested, int available, Guid? productId = null) =>
        new($"Insufficient stock. Requested: {requested}, Available: {available}", requested, available, productId);
    
    public static InvalidStockException ReservationFailed(int quantity, Guid productId) =>
        new($"Failed to reserve {quantity} units for product {productId}", quantity, null, productId);
    
    public static InvalidStockException InvalidAdjustment(int adjustment, int currentStock) =>
        new($"Stock adjustment of {adjustment} would result in negative stock (current: {currentStock})", 
            adjustment, currentStock);
}