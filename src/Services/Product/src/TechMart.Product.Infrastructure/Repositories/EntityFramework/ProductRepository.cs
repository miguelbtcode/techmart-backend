using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TechMart.Product.Domain.Product;
using TechMart.Product.Domain.Product.Enums;
using TechMart.Product.Domain.Product.ValueObjects;
using TechMart.Product.Infrastructure.Data.EntityFramework;
using TechMart.SharedKernel.Common;
using ProductEntity = TechMart.Product.Domain.Product.Product;

namespace TechMart.Product.Infrastructure.Repositories.EntityFramework;

public class ProductRepository : BaseRepository<ProductEntity, Guid>, IProductRepository
{
    public ProductRepository(ApplicationDbContext context) : base(context) { }

    public async Task<ProductEntity?> GetBySkuAsync(ProductSku sku, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(p => p.Sku.Value == sku.Value, cancellationToken);
    }

    public async Task<ProductEntity?> GetBySkuWithDetailsAsync(ProductSku sku, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(p => p.Images)
            .Include(p => p.Variants)
            .Include(p => p.Attributes)
            .Include(p => p.Reviews)
            .FirstOrDefaultAsync(p => p.Sku.Value == sku.Value, cancellationToken);
    }

    public async Task<ProductEntity?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(p => p.Images)
            .Include(p => p.Variants)
            .Include(p => p.Attributes)
            .Include(p => p.Reviews)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<bool> SkuExistsAsync(ProductSku sku, Guid? excludeProductId = null, CancellationToken cancellationToken = default)
    {
        var query = DbSet.Where(p => p.Sku.Value == sku.Value);
        
        if (excludeProductId.HasValue)
        {
            query = query.Where(p => p.Id != excludeProductId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<IEnumerable<ProductEntity>> GetByCategoryAsync(Guid categoryId, bool includeInactive = false, CancellationToken cancellationToken = default)
    {
        var query = DbSet.Where(p => p.CategoryId == categoryId);
        
        if (!includeInactive)
        {
            query = query.Where(p => p.IsActive);
        }

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ProductEntity>> GetByCategoriesAsync(IEnumerable<Guid> categoryIds, bool includeInactive = false, CancellationToken cancellationToken = default)
    {
        var query = DbSet.Where(p => categoryIds.Contains(p.CategoryId));
        
        if (!includeInactive)
        {
            query = query.Where(p => p.IsActive);
        }

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ProductEntity>> GetByBrandAsync(Guid brandId, bool includeInactive = false, CancellationToken cancellationToken = default)
    {
        var query = DbSet.Where(p => p.BrandId == brandId);
        
        if (!includeInactive)
        {
            query = query.Where(p => p.IsActive);
        }

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ProductEntity>> GetByBrandsAsync(IEnumerable<Guid> brandIds, bool includeInactive = false, CancellationToken cancellationToken = default)
    {
        var query = DbSet.Where(p => brandIds.Contains(p.BrandId));
        
        if (!includeInactive)
        {
            query = query.Where(p => p.IsActive);
        }

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ProductEntity>> GetByStatusAsync(ProductStatus status, CancellationToken cancellationToken = default)
    {
        return await DbSet.Where(p => p.Status == status).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ProductEntity>> GetActiveProductsAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.Where(p => p.Status == ProductStatus.Active && p.IsActive).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ProductEntity>> GetFeaturedProductsAsync(int limit = 10, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(p => p.IsFeatured && p.Status == ProductStatus.Active && p.IsActive)
            .OrderByDescending(p => p.CreatedAt)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ProductEntity>> SearchAsync(string searchText, bool includeInactive = false, CancellationToken cancellationToken = default)
    {
        var query = DbSet.AsQueryable();
        
        if (!string.IsNullOrWhiteSpace(searchText))
        {
            var lowerSearchText = searchText.ToLowerInvariant();
            query = query.Where(p => 
                p.Name.ToLower().Contains(lowerSearchText) ||
                p.Description.ToLower().Contains(lowerSearchText) ||
                (p.Tags != null && p.Tags.ToLower().Contains(lowerSearchText)));
        }
        
        if (!includeInactive)
        {
            query = query.Where(p => p.IsActive);
        }

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ProductEntity>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice, bool includeInactive = false, CancellationToken cancellationToken = default)
    {
        var query = DbSet.Where(p => p.Price.Amount >= minPrice && p.Price.Amount <= maxPrice);
        
        if (!includeInactive)
        {
            query = query.Where(p => p.IsActive);
        }

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ProductEntity>> GetOnSaleProductsAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(p => p.CompareAtPrice != null && 
                       p.CompareAtPrice.Amount > p.Price.Amount &&
                       p.Status == ProductStatus.Active && p.IsActive)
            .ToListAsync(cancellationToken);
    }

    // Paginated queries
    public async Task<PagedList<ProductEntity>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        return await PagedList<ProductEntity>.CreateAsync(DbSet, pageNumber, pageSize, cancellationToken);
    }

    public async Task<PagedList<ProductEntity>> GetPagedAsync(Expression<Func<ProductEntity, bool>> predicate, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = DbSet.Where(predicate);
        return await PagedList<ProductEntity>.CreateAsync(query, pageNumber, pageSize, cancellationToken);
    }

    public async Task<PagedList<ProductEntity>> GetPagedByCategoryAsync(Guid categoryId, int pageNumber, int pageSize, bool includeInactive = false, CancellationToken cancellationToken = default)
    {
        var query = DbSet.Where(p => p.CategoryId == categoryId);
        
        if (!includeInactive)
        {
            query = query.Where(p => p.IsActive);
        }

        return await PagedList<ProductEntity>.CreateAsync(query, pageNumber, pageSize, cancellationToken);
    }

    public async Task<PagedList<ProductEntity>> GetPagedByBrandAsync(Guid brandId, int pageNumber, int pageSize, bool includeInactive = false, CancellationToken cancellationToken = default)
    {
        var query = DbSet.Where(p => p.BrandId == brandId);
        
        if (!includeInactive)
        {
            query = query.Where(p => p.IsActive);
        }

        return await PagedList<ProductEntity>.CreateAsync(query, pageNumber, pageSize, cancellationToken);
    }

    // Statistics and analytics
    public async Task<int> GetTotalCountAsync(bool includeInactive = false, CancellationToken cancellationToken = default)
    {
        var query = DbSet.AsQueryable();
        
        if (!includeInactive)
        {
            query = query.Where(p => p.IsActive);
        }

        return await query.CountAsync(cancellationToken);
    }

    public async Task<Dictionary<ProductStatus, int>> GetCountByStatusAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .GroupBy(p => p.Status)
            .ToDictionaryAsync(g => g.Key, g => g.Count(), cancellationToken);
    }

    public async Task<int> GetCountByCategoryAsync(Guid categoryId, bool includeInactive = false, CancellationToken cancellationToken = default)
    {
        var query = DbSet.Where(p => p.CategoryId == categoryId);
        
        if (!includeInactive)
        {
            query = query.Where(p => p.IsActive);
        }

        return await query.CountAsync(cancellationToken);
    }

    public async Task<int> GetCountByBrandAsync(Guid brandId, bool includeInactive = false, CancellationToken cancellationToken = default)
    {
        var query = DbSet.Where(p => p.BrandId == brandId);
        
        if (!includeInactive)
        {
            query = query.Where(p => p.IsActive);
        }

        return await query.CountAsync(cancellationToken);
    }

    // Recent and trending
    public async Task<IEnumerable<ProductEntity>> GetRecentlyAddedAsync(int days = 7, int limit = 10, CancellationToken cancellationToken = default)
    {
        var cutoffDate = DateTime.UtcNow.AddDays(-days);
        return await DbSet
            .Where(p => p.CreatedAt >= cutoffDate && p.IsActive)
            .OrderByDescending(p => p.CreatedAt)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ProductEntity>> GetRecentlyUpdatedAsync(int days = 7, int limit = 10, CancellationToken cancellationToken = default)
    {
        var cutoffDate = DateTime.UtcNow.AddDays(-days);
        return await DbSet
            .Where(p => p.UpdatedAt.HasValue && p.UpdatedAt.Value >= cutoffDate && p.IsActive)
            .OrderByDescending(p => p.UpdatedAt)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ProductEntity>> GetTopRatedAsync(int limit = 10, int minReviewCount = 5, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(p => p.Reviews)
            .Where(p => p.Reviews.Count >= minReviewCount && p.IsActive)
            .OrderByDescending(p => p.Reviews.Where(r => r.Status == ReviewStatus.Approved).Average(r => r.Rating))
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    // Advanced queries
    public async Task<IEnumerable<ProductEntity>> GetLowStockProductsAsync(int threshold = 10, CancellationToken cancellationToken = default)
    {
        // This would require joining with inventory, simplified for now
        return await DbSet
            .Where(p => p.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ProductEntity>> GetProductsNeedingReviewAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(p => p.Images)
            .Where(p => p.IsActive && !p.Images.Any())
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ProductEntity>> GetBySpecificationAsync(Expression<Func<ProductEntity, bool>> specification, CancellationToken cancellationToken = default)
    {
        return await DbSet.Where(specification).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ProductEntity>> GetWithVariantsAsync(IEnumerable<Guid> productIds, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(p => p.Variants)
            .Where(p => productIds.Contains(p.Id))
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ProductEntity>> GetWithImagesAsync(IEnumerable<Guid> productIds, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(p => p.Images)
            .Where(p => productIds.Contains(p.Id))
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ProductEntity>> GetWithReviewsAsync(IEnumerable<Guid> productIds, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(p => p.Reviews)
            .Where(p => productIds.Contains(p.Id))
            .ToListAsync(cancellationToken);
    }

    // Bulk operations
    public async Task<int> BulkUpdateStatusAsync(IEnumerable<Guid> productIds, ProductStatus status, string? updatedBy = null, CancellationToken cancellationToken = default)
    {
        var products = await DbSet.Where(p => productIds.Contains(p.Id)).ToListAsync(cancellationToken);
        
        foreach (var product in products)
        {
            product.ChangeStatus(status, updatedBy);
        }

        return products.Count;
    }

    public async Task<int> BulkUpdateCategoryAsync(IEnumerable<Guid> productIds, Guid categoryId, string? updatedBy = null, CancellationToken cancellationToken = default)
    {
        var products = await DbSet.Where(p => productIds.Contains(p.Id)).ToListAsync(cancellationToken);
        
        foreach (var product in products)
        {
            product.ChangeCategory(categoryId, updatedBy);
        }

        return products.Count;
    }

    public async Task<int> BulkSetFeaturedAsync(IEnumerable<Guid> productIds, bool featured, string? updatedBy = null, CancellationToken cancellationToken = default)
    {
        var products = await DbSet.Where(p => productIds.Contains(p.Id)).ToListAsync(cancellationToken);
        
        foreach (var product in products)
        {
            product.SetFeatured(featured, updatedBy);
        }

        return products.Count;
    }
}