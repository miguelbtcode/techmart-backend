using System.Linq.Expressions;
using TechMart.Product.Domain.Product.Enums;

namespace TechMart.Product.Domain.Product;

/// <summary>
/// Contains specifications for product queries using the Specification pattern.
/// </summary>
public static class ProductSpecifications
{
    /// <summary>
    /// Specification for active products.
    /// </summary>
    /// <returns>Expression that filters active products.</returns>
    public static Expression<Func<Product, bool>> IsActive()
    {
        return product => product.Status == ProductStatus.Active && product.IsActive;
    }

    /// <summary>
    /// Specification for products by status.
    /// </summary>
    /// <param name="status">The product status to filter by.</param>
    /// <returns>Expression that filters products by status.</returns>
    public static Expression<Func<Product, bool>> HasStatus(ProductStatus status)
    {
        return product => product.Status == status;
    }

    /// <summary>
    /// Specification for products by category.
    /// </summary>
    /// <param name="categoryId">The category ID to filter by.</param>
    /// <returns>Expression that filters products by category.</returns>
    public static Expression<Func<Product, bool>> InCategory(Guid categoryId)
    {
        return product => product.CategoryId == categoryId;
    }

    /// <summary>
    /// Specification for products by brand.
    /// </summary>
    /// <param name="brandId">The brand ID to filter by.</param>
    /// <returns>Expression that filters products by brand.</returns>
    public static Expression<Func<Product, bool>> OfBrand(Guid brandId)
    {
        return product => product.BrandId == brandId;
    }

    /// <summary>
    /// Specification for products by type.
    /// </summary>
    /// <param name="productType">The product type to filter by.</param>
    /// <returns>Expression that filters products by type.</returns>
    public static Expression<Func<Product, bool>> OfType(ProductType productType)
    {
        return product => product.Type == productType;
    }

    /// <summary>
    /// Specification for featured products.
    /// </summary>
    /// <returns>Expression that filters featured products.</returns>
    public static Expression<Func<Product, bool>> IsFeatured()
    {
        return product => product.IsFeatured && product.Status == ProductStatus.Active;
    }

    /// <summary>
    /// Specification for products on sale.
    /// </summary>
    /// <returns>Expression that filters products that are on sale.</returns>
    public static Expression<Func<Product, bool>> IsOnSale()
    {
        return product => product.CompareAtPrice != null && 
                         product.CompareAtPrice.Amount > product.Price.Amount &&
                         product.Status == ProductStatus.Active;
    }

    /// <summary>
    /// Specification for products within a price range.
    /// </summary>
    /// <param name="minPrice">The minimum price.</param>
    /// <param name="maxPrice">The maximum price.</param>
    /// <returns>Expression that filters products within the price range.</returns>
    public static Expression<Func<Product, bool>> InPriceRange(decimal minPrice, decimal maxPrice)
    {
        return product => product.Price.Amount >= minPrice && product.Price.Amount <= maxPrice;
    }

    /// <summary>
    /// Specification for products with minimum rating.
    /// </summary>
    /// <param name="minRating">The minimum average rating.</param>
    /// <returns>Expression that filters products with minimum rating.</returns>
    public static Expression<Func<Product, bool>> HasMinimumRating(double minRating)
    {
        return product => product.Reviews.Any() && 
                         product.Reviews.Where(r => r.Status == ReviewStatus.Approved)
                                       .Average(r => r.Rating) >= minRating;
    }

    /// <summary>
    /// Specification for products with minimum review count.
    /// </summary>
    /// <param name="minReviewCount">The minimum number of reviews.</param>
    /// <returns>Expression that filters products with minimum review count.</returns>
    public static Expression<Func<Product, bool>> HasMinimumReviews(int minReviewCount)
    {
        return product => product.Reviews.Count(r => r.Status == ReviewStatus.Approved) >= minReviewCount;
    }

    /// <summary>
    /// Specification for products by SKU.
    /// </summary>
    /// <param name="sku">The SKU to search for.</param>
    /// <returns>Expression that filters products by SKU.</returns>
    public static Expression<Func<Product, bool>> HasSku(string sku)
    {
        return product => product.Sku.Value == sku.ToUpperInvariant();
    }

    /// <summary>
    /// Specification for products containing search text.
    /// </summary>
    /// <param name="searchText">The text to search for in name, description, or tags.</param>
    /// <returns>Expression that filters products containing the search text.</returns>
    public static Expression<Func<Product, bool>> ContainsText(string searchText)
    {
        var lowerSearchText = searchText.ToLowerInvariant();
        return product => product.Name.ToLower().Contains(lowerSearchText) ||
                         product.Description.ToLower().Contains(lowerSearchText) ||
                         (product.Tags != null && product.Tags.ToLower().Contains(lowerSearchText));
    }

    /// <summary>
    /// Specification for products created within a date range.
    /// </summary>
    /// <param name="startDate">The start date.</param>
    /// <param name="endDate">The end date.</param>
    /// <returns>Expression that filters products created within the date range.</returns>
    public static Expression<Func<Product, bool>> CreatedBetween(DateTime startDate, DateTime endDate)
    {
        return product => product.CreatedAt >= startDate && product.CreatedAt <= endDate;
    }

    /// <summary>
    /// Specification for products updated within a date range.
    /// </summary>
    /// <param name="startDate">The start date.</param>
    /// <param name="endDate">The end date.</param>
    /// <returns>Expression that filters products updated within the date range.</returns>
    public static Expression<Func<Product, bool>> UpdatedBetween(DateTime startDate, DateTime endDate)
    {
        return product => product.UpdatedAt.HasValue && 
                         product.UpdatedAt.Value >= startDate && 
                         product.UpdatedAt.Value <= endDate;
    }

    /// <summary>
    /// Specification for digital products.
    /// </summary>
    /// <returns>Expression that filters digital products.</returns>
    public static Expression<Func<Product, bool>> IsDigital()
    {
        return product => product.IsDigital;
    }

    /// <summary>
    /// Specification for physical products.
    /// </summary>
    /// <returns>Expression that filters physical products.</returns>
    public static Expression<Func<Product, bool>> IsPhysical()
    {
        return product => !product.IsDigital;
    }

    /// <summary>
    /// Specification for products that require shipping.
    /// </summary>
    /// <returns>Expression that filters products that require shipping.</returns>
    public static Expression<Func<Product, bool>> RequiresShipping()
    {
        return product => product.RequiresShipping;
    }

    /// <summary>
    /// Specification for products with variants.
    /// </summary>
    /// <returns>Expression that filters products that have variants.</returns>
    public static Expression<Func<Product, bool>> HasVariants()
    {
        return product => product.Variants.Any();
    }

    /// <summary>
    /// Specification for products with images.
    /// </summary>
    /// <returns>Expression that filters products that have images.</returns>
    public static Expression<Func<Product, bool>> HasImages()
    {
        return product => product.Images.Any();
    }

    /// <summary>
    /// Specification for products with a specific attribute.
    /// </summary>
    /// <param name="attributeName">The attribute name.</param>
    /// <param name="attributeValue">The attribute value (optional).</param>
    /// <returns>Expression that filters products with the specified attribute.</returns>
    public static Expression<Func<Product, bool>> HasAttribute(string attributeName, string? attributeValue = null)
    {
        if (string.IsNullOrEmpty(attributeValue))
        {
            return product => product.Attributes.Any(a => a.Name == attributeName);
        }
        
        return product => product.Attributes.Any(a => a.Name == attributeName && a.Value == attributeValue);
    }

    /// <summary>
    /// Specification for products created by a specific user.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <returns>Expression that filters products created by the user.</returns>
    public static Expression<Func<Product, bool>> CreatedBy(string userId)
    {
        return product => product.CreatedBy == userId;
    }

    /// <summary>
    /// Specification for products updated by a specific user.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <returns>Expression that filters products updated by the user.</returns>
    public static Expression<Func<Product, bool>> UpdatedBy(string userId)
    {
        return product => product.UpdatedBy == userId;
    }

    /// <summary>
    /// Specification for products with weight between specified values.
    /// </summary>
    /// <param name="minWeight">The minimum weight in kg.</param>
    /// <param name="maxWeight">The maximum weight in kg.</param>
    /// <returns>Expression that filters products within the weight range.</returns>
    public static Expression<Func<Product, bool>> WeightBetween(decimal minWeight, decimal maxWeight)
    {
        return product => product.Weight != null && 
                         product.Weight.Value >= minWeight && 
                         product.Weight.Value <= maxWeight;
    }

    /// <summary>
    /// Specification for products with high discount percentage.
    /// </summary>
    /// <param name="minDiscountPercentage">The minimum discount percentage.</param>
    /// <returns>Expression that filters products with high discount.</returns>
    public static Expression<Func<Product, bool>> HasMinimumDiscount(decimal minDiscountPercentage)
    {
        return product => product.CompareAtPrice != null && 
                         product.CompareAtPrice.Amount > product.Price.Amount &&
                         ((1 - (product.Price.Amount / product.CompareAtPrice.Amount)) * 100) >= minDiscountPercentage;
    }

    /// <summary>
    /// Specification for recently added products.
    /// </summary>
    /// <param name="days">Number of days to consider as recent.</param>
    /// <returns>Expression that filters recently added products.</returns>
    public static Expression<Func<Product, bool>> AddedRecently(int days = 7)
    {
        var cutoffDate = DateTime.UtcNow.AddDays(-days);
        return product => product.CreatedAt >= cutoffDate;
    }

    /// <summary>
    /// Specification for products with approved reviews only.
    /// </summary>
    /// <returns>Expression that includes products that have at least one approved review.</returns>
    public static Expression<Func<Product, bool>> HasApprovedReviews()
    {
        return product => product.Reviews.Any(r => r.Status == ReviewStatus.Approved);
    }
}