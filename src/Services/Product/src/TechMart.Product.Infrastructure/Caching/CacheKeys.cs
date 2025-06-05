namespace TechMart.Product.Infrastructure.Caching;

public static class CacheKeys
{
    private const string BASE_KEY = "techmart:product:";
    
    // Product cache keys
    public static string Product(Guid productId) => $"{BASE_KEY}products:{productId}";
    public static string ProductBySku(string sku) => $"{BASE_KEY}products:sku:{sku}";
    public static string ProductsByCategory(Guid categoryId) => $"{BASE_KEY}products:category:{categoryId}";
    public static string ProductsByBrand(Guid brandId) => $"{BASE_KEY}products:brand:{brandId}";
    public static string FeaturedProducts() => $"{BASE_KEY}products:featured";
    
    // Brand cache keys
    public static string Brand(Guid brandId) => $"{BASE_KEY}brands:{brandId}";
    public static string BrandBySlug(string slug) => $"{BASE_KEY}brands:slug:{slug}";
    public static string ActiveBrands() => $"{BASE_KEY}brands:active";
    
    // Category cache keys
    public static string Category(Guid categoryId) => $"{BASE_KEY}categories:{categoryId}";
    public static string CategoryBySlug(string slug) => $"{BASE_KEY}categories:slug:{slug}";
    public static string ActiveCategories() => $"{BASE_KEY}categories:active";
    public static string CategoriesByParent(Guid? parentId) => $"{BASE_KEY}categories:parent:{parentId ?? Guid.Empty}";
    
    // Inventory cache keys
    public static string Inventory(Guid productId) => $"{BASE_KEY}inventory:product:{productId}";
    public static string LowStockProducts() => $"{BASE_KEY}inventory:lowstock";
    
    // Search cache keys
    public static string SearchResults(string searchTerm, int page, int size) => 
        $"{BASE_KEY}search:{searchTerm.ToLowerInvariant()}:p{page}:s{size}";
}