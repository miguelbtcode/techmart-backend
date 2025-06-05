using System.Linq.Expressions;
using TechMart.Product.Domain.Product.Enums;
using TechMart.Product.Domain.Product.ValueObjects;
using TechMart.SharedKernel.Abstractions;
using TechMart.SharedKernel.Common;

namespace TechMart.Product.Domain.Product;

/// <summary>
/// Repository interface for the Product aggregate root.
/// Defines methods for persisting and retrieving products.
/// </summary>
public interface IProductRepository : IRepository<Product, Guid>
{
    // Specific product queries

    /// <summary>
    /// Gets a product by its SKU.
    /// </summary>
    /// <param name="sku">The product SKU.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The product if found; otherwise, null.</returns>
    Task<Product?> GetBySkuAsync(ProductSku sku, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a product by its SKU with all related entities loaded.
    /// </summary>
    /// <param name="sku">The product SKU.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The product with all related data if found; otherwise, null.</returns>
    Task<Product?> GetBySkuWithDetailsAsync(ProductSku sku, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a product by ID with all related entities loaded.
    /// </summary>
    /// <param name="id">The product ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The product with all related data if found; otherwise, null.</returns>
    Task<Product?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a SKU already exists.
    /// </summary>
    /// <param name="sku">The SKU to check.</param>
    /// <param name="excludeProductId">Product ID to exclude from the check (for updates).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if SKU exists; otherwise, false.</returns>
    Task<bool> SkuExistsAsync(ProductSku sku, Guid? excludeProductId = null, CancellationToken cancellationToken = default);

    // Category-based queries

    /// <summary>
    /// Gets products by category ID.
    /// </summary>
    /// <param name="categoryId">The category ID.</param>
    /// <param name="includeInactive">Whether to include inactive products.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of products in the category.</returns>
    Task<IEnumerable<Product>> GetByCategoryAsync(Guid categoryId, bool includeInactive = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets products by multiple category IDs.
    /// </summary>
    /// <param name="categoryIds">The category IDs.</param>
    /// <param name="includeInactive">Whether to include inactive products.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of products in the categories.</returns>
    Task<IEnumerable<Product>> GetByCategoriesAsync(IEnumerable<Guid> categoryIds, bool includeInactive = false, CancellationToken cancellationToken = default);

    // Brand-based queries

    /// <summary>
    /// Gets products by brand ID.
    /// </summary>
    /// <param name="brandId">The brand ID.</param>
    /// <param name="includeInactive">Whether to include inactive products.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of products from the brand.</returns>
    Task<IEnumerable<Product>> GetByBrandAsync(Guid brandId, bool includeInactive = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets products by multiple brand IDs.
    /// </summary>
    /// <param name="brandIds">The brand IDs.</param>
    /// <param name="includeInactive">Whether to include inactive products.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of products from the brands.</returns>
    Task<IEnumerable<Product>> GetByBrandsAsync(IEnumerable<Guid> brandIds, bool includeInactive = false, CancellationToken cancellationToken = default);

    // Status-based queries

    /// <summary>
    /// Gets products by status.
    /// </summary>
    /// <param name="status">The product status.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of products with the specified status.</returns>
    Task<IEnumerable<Product>> GetByStatusAsync(ProductStatus status, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets active products only.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of active products.</returns>
    Task<IEnumerable<Product>> GetActiveProductsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets featured products.
    /// </summary>
    /// <param name="limit">Maximum number of products to return.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of featured products.</returns>
    Task<IEnumerable<Product>> GetFeaturedProductsAsync(int limit = 10, CancellationToken cancellationToken = default);

    // Search and filtering

    /// <summary>
    /// Searches products by text in name, description, or tags.
    /// </summary>
    /// <param name="searchText">The text to search for.</param>
    /// <param name="includeInactive">Whether to include inactive products.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of products matching the search criteria.</returns>
    Task<IEnumerable<Product>> SearchAsync(string searchText, bool includeInactive = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets products within a price range.
    /// </summary>
    /// <param name="minPrice">The minimum price.</param>
    /// <param name="maxPrice">The maximum price.</param>
    /// <param name="includeInactive">Whether to include inactive products.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of products within the price range.</returns>
    Task<IEnumerable<Product>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice, bool includeInactive = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets products that are on sale.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of products on sale.</returns>
    Task<IEnumerable<Product>> GetOnSaleProductsAsync(CancellationToken cancellationToken = default);

    // Paginated queries

    /// <summary>
    /// Gets a paginated list of products.
    /// </summary>
    /// <param name="pageNumber">The page number (1-based).</param>
    /// <param name="pageSize">The page size.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paginated list of products.</returns>
    Task<PagedList<Product>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a paginated list of products with filtering.
    /// </summary>
    /// <param name="predicate">The filter predicate.</param>
    /// <param name="pageNumber">The page number (1-based).</param>
    /// <param name="pageSize">The page size.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paginated list of filtered products.</returns>
    Task<PagedList<Product>> GetPagedAsync(Expression<Func<Product, bool>> predicate, int pageNumber, int pageSize, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a paginated list of products by category.
    /// </summary>
    /// <param name="categoryId">The category ID.</param>
    /// <param name="pageNumber">The page number (1-based).</param>
    /// <param name="pageSize">The page size.</param>
    /// <param name="includeInactive">Whether to include inactive products.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paginated list of products in the category.</returns>
    Task<PagedList<Product>> GetPagedByCategoryAsync(Guid categoryId, int pageNumber, int pageSize, bool includeInactive = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a paginated list of products by brand.
    /// </summary>
    /// <param name="brandId">The brand ID.</param>
    /// <param name="pageNumber">The page number (1-based).</param>
    /// <param name="pageSize">The page size.</param>
    /// <param name="includeInactive">Whether to include inactive products.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paginated list of products from the brand.</returns>
    Task<PagedList<Product>> GetPagedByBrandAsync(Guid brandId, int pageNumber, int pageSize, bool includeInactive = false, CancellationToken cancellationToken = default);

    // Statistics and analytics

    /// <summary>
    /// Gets the total count of products.
    /// </summary>
    /// <param name="includeInactive">Whether to include inactive products.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The total number of products.</returns>
    Task<int> GetTotalCountAsync(bool includeInactive = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets product count by status.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A dictionary with status counts.</returns>
    Task<Dictionary<ProductStatus, int>> GetCountByStatusAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets product count by category.
    /// </summary>
    /// <param name="categoryId">The category ID.</param>
    /// <param name="includeInactive">Whether to include inactive products.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The number of products in the category.</returns>
    Task<int> GetCountByCategoryAsync(Guid categoryId, bool includeInactive = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets product count by brand.
    /// </summary>
    /// <param name="brandId">The brand ID.</param>
    /// <param name="includeInactive">Whether to include inactive products.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The number of products from the brand.</returns>
    Task<int> GetCountByBrandAsync(Guid brandId, bool includeInactive = false, CancellationToken cancellationToken = default);

    // Recent and trending

    /// <summary>
    /// Gets recently added products.
    /// </summary>
    /// <param name="days">Number of days to consider as recent.</param>
    /// <param name="limit">Maximum number of products to return.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of recently added products.</returns>
    Task<IEnumerable<Product>> GetRecentlyAddedAsync(int days = 7, int limit = 10, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets recently updated products.
    /// </summary>
    /// <param name="days">Number of days to consider as recent.</param>
    /// <param name="limit">Maximum number of products to return.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of recently updated products.</returns>
    Task<IEnumerable<Product>> GetRecentlyUpdatedAsync(int days = 7, int limit = 10, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets top-rated products.
    /// </summary>
    /// <param name="limit">Maximum number of products to return.</param>
    /// <param name="minReviewCount">Minimum number of reviews required.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of top-rated products.</returns>
    Task<IEnumerable<Product>> GetTopRatedAsync(int limit = 10, int minReviewCount = 5, CancellationToken cancellationToken = default);

    // Advanced queries

    /// <summary>
    /// Gets products with low stock (requires integration with inventory).
    /// </summary>
    /// <param name="threshold">The stock threshold.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of products with low stock.</returns>
    Task<IEnumerable<Product>> GetLowStockProductsAsync(int threshold = 10, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets products that need review (no images, incomplete data, etc.).
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of products that need review.</returns>
    Task<IEnumerable<Product>> GetProductsNeedingReviewAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets products by multiple criteria using specifications.
    /// </summary>
    /// <param name="specification">The specification to apply.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of products matching the specification.</returns>
    Task<IEnumerable<Product>> GetBySpecificationAsync(Expression<Func<Product, bool>> specification, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets products with their variants.
    /// </summary>
    /// <param name="productIds">The product IDs.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Products with their variants loaded.</returns>
    Task<IEnumerable<Product>> GetWithVariantsAsync(IEnumerable<Guid> productIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets products with their images.
    /// </summary>
    /// <param name="productIds">The product IDs.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Products with their images loaded.</returns>
    Task<IEnumerable<Product>> GetWithImagesAsync(IEnumerable<Guid> productIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets products with their reviews.
    /// </summary>
    /// <param name="productIds">The product IDs.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Products with their reviews loaded.</returns>
    Task<IEnumerable<Product>> GetWithReviewsAsync(IEnumerable<Guid> productIds, CancellationToken cancellationToken = default);

    // Bulk operations

    /// <summary>
    /// Bulk updates product status.
    /// </summary>
    /// <param name="productIds">The product IDs to update.</param>
    /// <param name="status">The new status.</param>
    /// <param name="updatedBy">The user performing the update.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The number of products updated.</returns>
    Task<int> BulkUpdateStatusAsync(IEnumerable<Guid> productIds, ProductStatus status, string? updatedBy = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Bulk updates product category.
    /// </summary>
    /// <param name="productIds">The product IDs to update.</param>
    /// <param name="categoryId">The new category ID.</param>
    /// <param name="updatedBy">The user performing the update.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The number of products updated.</returns>
    Task<int> BulkUpdateCategoryAsync(IEnumerable<Guid> productIds, Guid categoryId, string? updatedBy = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Bulk sets featured flag for products.
    /// </summary>
    /// <param name="productIds">The product IDs to update.</param>
    /// <param name="featured">Whether to set as featured.</param>
    /// <param name="updatedBy">The user performing the update.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The number of products updated.</returns>
    Task<int> BulkSetFeaturedAsync(IEnumerable<Guid> productIds, bool featured, string? updatedBy = null, CancellationToken cancellationToken = default);
}