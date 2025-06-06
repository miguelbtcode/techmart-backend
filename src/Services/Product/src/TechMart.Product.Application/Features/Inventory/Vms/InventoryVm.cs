using TechMart.Product.Domain.Inventory.Enums;

namespace TechMart.Product.Application.Features.Inventory.Vms;

public record InventoryVm
{
    public Guid Id { get; init; }
    public Guid ProductId { get; init; }
    public string ProductSku { get; init; } = string.Empty;
    public int QuantityOnHand { get; init; }
    public int QuantityReserved { get; init; }
    public int AvailableQuantity { get; init; }
    public int ReorderLevel { get; init; }
    public int MaxStockLevel { get; init; }
    public InventoryStatus Status { get; init; }
    public bool IsLowStock { get; init; }
    public bool IsOutOfStock { get; init; }
    public DateTime LastUpdated { get; init; }
}