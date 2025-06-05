using Microsoft.EntityFrameworkCore;
using TechMart.Product.Domain.Brand;

namespace TechMart.Product.Infrastructure.Data.EntityFramework.Seeders;

public static class BrandSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        if (await context.Brands.AnyAsync())
        {
            return; // Data already seeded
        }

        var brands = new List<Brand>
        {
            CreateBrand("Apple", "Innovative technology products", "apple", "https://example.com/logos/apple.png", "https://apple.com"),
            CreateBrand("Samsung", "Leading electronics manufacturer", "samsung", "https://example.com/logos/samsung.png", "https://samsung.com"),
            CreateBrand("Sony", "Entertainment and electronics company", "sony", "https://example.com/logos/sony.png", "https://sony.com"),
            CreateBrand("Microsoft", "Software and technology solutions", "microsoft", "https://example.com/logos/microsoft.png", "https://microsoft.com"),
            CreateBrand("Google", "Search and cloud technology", "google", "https://example.com/logos/google.png", "https://google.com"),
            CreateBrand("Amazon", "E-commerce and cloud services", "amazon", "https://example.com/logos/amazon.png", "https://amazon.com"),
            CreateBrand("Dell", "Computer technology solutions", "dell", "https://example.com/logos/dell.png", "https://dell.com"),
            CreateBrand("HP", "Personal systems and printing", "hp", "https://example.com/logos/hp.png", "https://hp.com"),
            CreateBrand("Lenovo", "Intelligent transformation technology", "lenovo", "https://example.com/logos/lenovo.png", "https://lenovo.com"),
            CreateBrand("Intel", "Semiconductor and computing innovation", "intel", "https://example.com/logos/intel.png", "https://intel.com")
        };

        context.Brands.AddRange(brands);
        await context.SaveChangesAsync();
    }

    private static Brand CreateBrand(string name, string description, string slug, string logoUrl, string websiteUrl)
    {
        var brand = new Brand(name, description);
        brand.SetLogo(logoUrl);
        brand.SetWebsite(websiteUrl);
        return brand;
    }
}