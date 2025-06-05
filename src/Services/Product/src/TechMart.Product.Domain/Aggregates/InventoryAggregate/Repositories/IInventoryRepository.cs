using TechMart.Product.Domain.Aggregates.InventoryAggregate.Entities;
using TechMart.SharedKernel.Abstractions;

namespace TechMart.Product.Domain.Aggregates.InventoryAggregate.Repositories;

public interface IInventoryRepository : IRepository<Inventory, Guid>
{
    Task<Inventory?> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<Inventory?> GetByProductSkuAsync(string productSku, CancellationToken cancellationToken = default);
    Task<IEnumerable<Inventory>> GetLowStockAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Inventory>> GetOutOfStockAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Inventory>> GetByProductIdsAsync(IEnumerable<Guid> productIds, CancellationToken cancellationToken = default);
}