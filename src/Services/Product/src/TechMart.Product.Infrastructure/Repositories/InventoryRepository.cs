using Microsoft.EntityFrameworkCore;
using TechMart.Product.Domain.Inventory;
using TechMart.Product.Infrastructure.Data;

namespace TechMart.Product.Infrastructure.Repositories;

public class InventoryRepository : BaseRepository<Inventory, Guid>, IInventoryRepository
{
    public InventoryRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Inventory?> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(i => i.Transactions)
            .FirstOrDefaultAsync(i => i.ProductId == productId, cancellationToken);
    }

    public async Task<Inventory?> GetByProductSkuAsync(string productSku, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(i => i.Transactions)
            .FirstOrDefaultAsync(i => i.ProductSku == productSku, cancellationToken);
    }

    public async Task<IEnumerable<Inventory>> GetLowStockAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(i => i.QuantityOnHand <= i.ReorderLevel)
            .OrderBy(i => i.QuantityOnHand)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Inventory>> GetOutOfStockAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(i => i.QuantityOnHand <= 0)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Inventory>> GetByProductIdsAsync(IEnumerable<Guid> productIds, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(i => productIds.Contains(i.ProductId))
            .ToListAsync(cancellationToken);
    }
}