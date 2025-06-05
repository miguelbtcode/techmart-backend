using TechMart.Product.Domain.Enums;
using TechMart.SharedKernel.Base;

namespace TechMart.Product.Domain.Aggregates.InventoryAggregate.Entities;

public class Inventory : BaseAggregateRoot<Guid>
{
    private readonly List<InventoryTransaction> _transactions = new();

    public Guid ProductId { get; private set; }
    public string ProductSku { get; private set; }
    public int QuantityOnHand { get; private set; }
    public int QuantityReserved { get; private set; }
    public int ReorderLevel { get; private set; }
    public int MaxStockLevel { get; private set; }
    public InventoryStatus Status { get; private set; }
    public DateTime LastUpdated { get; private set; }

    public IReadOnlyList<InventoryTransaction> Transactions => _transactions.AsReadOnly();

    // Calculated properties
    public int AvailableQuantity => QuantityOnHand - QuantityReserved;
    public bool IsLowStock => QuantityOnHand <= ReorderLevel;
    public bool IsOutOfStock => QuantityOnHand <= 0;

    // Private constructor for EF Core
    private Inventory() { }

    public Inventory(Guid productId, string productSku, int initialQuantity = 0)
    {
        Id = Guid.NewGuid();
        ProductId = productId;
        ProductSku = Guard.NotNullOrWhiteSpace(productSku, nameof(productSku));
        QuantityOnHand = Math.Max(0, initialQuantity);
        QuantityReserved = 0;
        ReorderLevel = 10;
        MaxStockLevel = 1000;
        Status = DetermineStatus();
        LastUpdated = DateTime.UtcNow;

        if (initialQuantity > 0)
        {
            AddTransaction(TransactionType.StockIn, initialQuantity, "Initial stock");
        }
    }

    public void AdjustStock(int quantity, string reason)
    {
        var oldQuantity = QuantityOnHand;
        QuantityOnHand = Math.Max(0, QuantityOnHand + quantity);
        
        var transactionType = quantity > 0 ? TransactionType.StockIn : TransactionType.StockOut;
        AddTransaction(transactionType, Math.Abs(quantity), reason);
        
        UpdateStatus();
    }

    public bool TryReserveStock(int quantity)
    {
        if (AvailableQuantity >= quantity)
        {
            QuantityReserved += quantity;
            AddTransaction(TransactionType.Reserved, quantity, "Stock reserved");
            UpdateStatus();
            return true;
        }
        return false;
    }

    public void ReleaseReservedStock(int quantity)
    {
        var releaseAmount = Math.Min(quantity, QuantityReserved);
        QuantityReserved -= releaseAmount;
        AddTransaction(TransactionType.Released, releaseAmount, "Stock released");
        UpdateStatus();
    }

    public void SetReorderLevel(int level)
    {
        ReorderLevel = Math.Max(0, level);
        UpdateStatus();
    }

    private void AddTransaction(TransactionType type, int quantity, string reason)
    {
        var transaction = new InventoryTransaction(Id, type, quantity, reason);
        _transactions.Add(transaction);
        LastUpdated = DateTime.UtcNow;
    }

    private void UpdateStatus()
    {
        Status = DetermineStatus();
        LastUpdated = DateTime.UtcNow;
    }

    private InventoryStatus DetermineStatus()
    {
        if (QuantityOnHand <= 0) return InventoryStatus.OutOfStock;
        if (QuantityOnHand <= ReorderLevel) return InventoryStatus.LowStock;
        return InventoryStatus.InStock;
    }

    private static class Guard
    {
        public static string NotNullOrWhiteSpace(string value, string paramName)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"{paramName} cannot be null or empty", paramName);
            return value;
        }
    }
}