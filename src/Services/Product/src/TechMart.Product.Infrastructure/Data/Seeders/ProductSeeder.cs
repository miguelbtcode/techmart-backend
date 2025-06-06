using Microsoft.EntityFrameworkCore;
using TechMart.Product.Domain.Product.Enums;
using TechMart.Product.Domain.Product.ValueObjects;
using ProductEntity = TechMart.Product.Domain.Product.Product;

namespace TechMart.Product.Infrastructure.Data.Seeders;

public static class ProductSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        if (await context.Products.AnyAsync())
        {
            return; // Data already seeded
        }

        // Get some brands and categories for reference
        var appleBrand = await context.Brands.FirstAsync(b => b.Name == "Apple");
        var samsungBrand = await context.Brands.FirstAsync(b => b.Name == "Samsung");
        var sonyBrand = await context.Brands.FirstAsync(b => b.Name == "Sony");
        var microsoftBrand = await context.Brands.FirstAsync(b => b.Name == "Microsoft");

        var laptopCategory = await context.Categories.FirstAsync(c => c.Name == "Laptops");
        var phoneCategory = await context.Categories.FirstAsync(c => c.Name == "Smartphones");
        var tabletCategory = await context.Categories.FirstAsync(c => c.Name == "Tablets");
        var gamingCategory = await context.Categories.FirstAsync(c => c.Name == "Gaming Consoles");

        var products = new List<ProductEntity>();

        // Apple Products
        var macbookPro = CreateProduct(
            "MBP16M3", "MacBook Pro 16-inch M3", 
            "Powerful laptop with M3 chip, perfect for professionals and creators",
            2499.00m, "USD", appleBrand.Id, laptopCategory.Id, ProductType.Physical);
        macbookPro.UpdatePhysicalProperties(new Weight(2.1m, "kg"), new Dimensions(35.57m, 24.81m, 1.68m, "cm"));
        macbookPro.SetFeatured(true);
        products.Add(macbookPro);

        var iphone15Pro = CreateProduct(
            "IP15PRO", "iPhone 15 Pro", 
            "Latest iPhone with titanium design and advanced camera system",
            999.00m, "USD", appleBrand.Id, phoneCategory.Id, ProductType.Physical);
        iphone15Pro.UpdatePhysicalProperties(new Weight(0.187m, "kg"), new Dimensions(14.67m, 7.09m, 0.83m, "cm"));
        iphone15Pro.SetFeatured(true);
        products.Add(iphone15Pro);

        var ipadPro = CreateProduct(
            "IPADPRO13", "iPad Pro 13-inch", 
            "Ultimate iPad experience with M4 chip and stunning display",
            1299.00m, "USD", appleBrand.Id, tabletCategory.Id, ProductType.Physical);
        ipadPro.UpdatePhysicalProperties(new Weight(0.682m, "kg"), new Dimensions(28.05m, 21.49m, 0.51m, "cm"));
        products.Add(ipadPro);

        // Samsung Products
        var galaxyS24 = CreateProduct(
            "GS24ULTRA", "Galaxy S24 Ultra", 
            "Samsung's flagship phone with S Pen and advanced AI features",
            1199.00m, "USD", samsungBrand.Id, phoneCategory.Id, ProductType.Physical);
        galaxyS24.UpdatePhysicalProperties(new Weight(0.232m, "kg"), new Dimensions(16.26m, 7.90m, 0.86m, "cm"));
        galaxyS24.SetFeatured(true);
        products.Add(galaxyS24);

        var galaxyBook = CreateProduct(
            "GBPRO360", "Galaxy Book Pro 360", 
            "Versatile 2-in-1 laptop with stunning AMOLED display",
            1399.00m, "USD", samsungBrand.Id, laptopCategory.Id, ProductType.Physical);
        galaxyBook.UpdatePhysicalProperties(new Weight(1.39m, "kg"), new Dimensions(30.41m, 19.65m, 1.19m, "cm"));
        products.Add(galaxyBook);

        // Sony Products
        var ps5 = CreateProduct(
            "PS5STD", "PlayStation 5", 
            "Next-generation gaming console with incredible graphics and speed",
            499.00m, "USD", sonyBrand.Id, gamingCategory.Id, ProductType.Physical);
        ps5.UpdatePhysicalProperties(new Weight(4.5m, "kg"), new Dimensions(39.0m, 26.0m, 10.4m, "cm"));
        ps5.SetFeatured(true);
        products.Add(ps5);

        // Microsoft Products
        var surfacePro = CreateProduct(
            "SURFPRO9", "Surface Pro 9", 
            "Versatile 2-in-1 tablet that transforms into a laptop",
            999.00m, "USD", microsoftBrand.Id, tabletCategory.Id, ProductType.Physical);
        surfacePro.UpdatePhysicalProperties(new Weight(0.879m, "kg"), new Dimensions(28.7m, 20.9m, 0.93m, "cm"));
        products.Add(surfacePro);

        var xboxSeriesX = CreateProduct(
            "XBXSX", "Xbox Series X", 
            "Most powerful Xbox console with 4K gaming and quick resume",
            499.00m, "USD", microsoftBrand.Id, gamingCategory.Id, ProductType.Physical);
        xboxSeriesX.UpdatePhysicalProperties(new Weight(4.45m, "kg"), new Dimensions(30.1m, 15.1m, 15.1m, "cm"));
        products.Add(xboxSeriesX);

        // Add some product images
        AddProductImages(macbookPro);
        AddProductImages(iphone15Pro);
        AddProductImages(galaxyS24);
        AddProductImages(ps5);

        // Add some product attributes
        AddProductAttributes(macbookPro);
        AddProductAttributes(iphone15Pro);
        AddProductAttributes(galaxyS24);
        AddProductAttributes(ps5);

        // Add some variants
        AddProductVariants(iphone15Pro);
        AddProductVariants(galaxyS24);

        context.Products.AddRange(products);
        await context.SaveChangesAsync();
    }

    private static ProductEntity CreateProduct(
        string sku, string name, string description, 
        decimal price, string currency, 
        Guid brandId, Guid categoryId, ProductType type)
    {
        var productPriceResult = Price.Create(price, currency);
        var productPrice = productPriceResult.Value;

        var productSkuResult = ProductSku.Create(sku);
        var productSku = productSkuResult.Value;
        
        var product = new ProductEntity(
            productSku, name, description, 
            productPrice, brandId, categoryId, type);
        
        // Activate the product
        product.Activate();
        
        return product;
    }

    private static void AddProductImages(ProductEntity product)
    {
        var baseUrl = "https://example.com/images/products";
        var productSlug = product.Name.ToLowerInvariant().Replace(" ", "-");

        var imageUrlFirstResult = ImageUrl.Create($"{baseUrl}/{productSlug}/main.jpg");
        var imageUrlSecondResult = ImageUrl.Create($"{baseUrl}/{productSlug}/side.jpg");
        var imageUrlThirdResult = ImageUrl.Create($"{baseUrl}/{productSlug}/back.jpg");
        
        product.AddImage(imageUrlFirstResult.Value, $"{product.Name} main image", true);
        product.AddImage(imageUrlSecondResult.Value, $"{product.Name} side view", false);
        product.AddImage(imageUrlThirdResult.Value, $"{product.Name} back view", false);
    }

    private static void AddProductAttributes(ProductEntity product)
    {
        switch (product.Name)
        {
            case "MacBook Pro 16-inch M3":
                product.SetAttribute("Processor", "Apple M3 Pro");
                product.SetAttribute("Memory", "18GB Unified Memory");
                product.SetAttribute("Storage", "512GB SSD");
                product.SetAttribute("Display", "16.2-inch Liquid Retina XDR");
                product.SetAttribute("Battery Life", "Up to 22 hours");
                break;

            case "iPhone 15 Pro":
                product.SetAttribute("Processor", "A17 Pro chip");
                product.SetAttribute("Storage", "128GB");
                product.SetAttribute("Display", "6.1-inch Super Retina XDR");
                product.SetAttribute("Camera", "48MP Main camera");
                product.SetAttribute("Material", "Titanium");
                break;

            case "Galaxy S24 Ultra":
                product.SetAttribute("Processor", "Snapdragon 8 Gen 3");
                product.SetAttribute("Storage", "256GB");
                product.SetAttribute("Display", "6.8-inch Dynamic AMOLED 2X");
                product.SetAttribute("Camera", "200MP Main camera");
                product.SetAttribute("S Pen", "Included");
                break;

            case "PlayStation 5":
                product.SetAttribute("Processor", "AMD Zen 2");
                product.SetAttribute("Graphics", "AMD RDNA 2");
                product.SetAttribute("Storage", "825GB SSD");
                product.SetAttribute("Resolution", "Up to 4K");
                product.SetAttribute("Ray Tracing", "Hardware-accelerated");
                break;
        }
    }

    private static void AddProductVariants(ProductEntity product)
    {
        if (product.Name == "iPhone 15 Pro")
        {
            var attributes128 = new Dictionary<string, string> { { "Storage", "128GB" }, { "Color", "Natural Titanium" } };
            var attributes256 = new Dictionary<string, string> { { "Storage", "256GB" }, { "Color", "Natural Titanium" } };
            var attributes512 = new Dictionary<string, string> { { "Storage", "512GB" }, { "Color", "Natural Titanium" } };

            
            product.AddVariant(ProductSku.Create("IP15PRO128").Value, "iPhone 15 Pro 128GB", Price.Create(999.00m, "USD").Value, attributes128);
            product.AddVariant(ProductSku.Create("IP15PRO256").Value, "iPhone 15 Pro 256GB", Price.Create(1099.00m, "USD").Value, attributes256);
            product.AddVariant(ProductSku.Create("IP15PRO512").Value, "iPhone 15 Pro 512GB", Price.Create(1299.00m, "USD").Value, attributes512);
        }

        if (product.Name == "Galaxy S24 Ultra")
        {
            var attributes256 = new Dictionary<string, string> { { "Storage", "256GB" }, { "Color", "Titanium Black" } };
            var attributes512 = new Dictionary<string, string> { { "Storage", "512GB" }, { "Color", "Titanium Black" } };
            var attributes1TB = new Dictionary<string, string> { { "Storage", "1TB" }, { "Color", "Titanium Black" } };

            product.AddVariant(ProductSku.Create("GS24U256").Value, "Galaxy S24 Ultra 256GB", Price.Create(1199.00m, "USD").Value, attributes256);
            product.AddVariant(ProductSku.Create("GS24U512").Value, "Galaxy S24 Ultra 512GB", Price.Create(1399.00m, "USD").Value, attributes512);
            product.AddVariant(ProductSku.Create("GS24U1TB").Value, "Galaxy S24 Ultra 1TB", Price.Create(1599.00m, "USD").Value, attributes1TB);
        }
    }
}