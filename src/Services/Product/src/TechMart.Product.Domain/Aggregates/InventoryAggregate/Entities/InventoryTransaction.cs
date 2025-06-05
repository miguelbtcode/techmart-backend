using TechMart.Product.Domain.Enums;
using TechMart.SharedKernel.Base;

namespace TechMart.Product.Domain.Aggregates.InventoryAggregate.Entities;

public class InventoryTransaction : BaseEntity<Guid>
{
    public Guid InventoryId { get; private set; }
    public TransactionType Type { get; private set; }
    public int Quantity { get; private set; }
    public string Reason { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // Private constructor for EF Core
    private InventoryTransaction() { }

    internal InventoryTransaction(Guid inventoryId, TransactionType type, int quantity, string reason)
    {
        Id = Guid.NewGuid();
        InventoryId = inventoryId;
        Type = type;
        Quantity = quantity;
        Reason = reason ?? string.Empty;
        CreatedAt = DateTime.UtcNow;
    }
}