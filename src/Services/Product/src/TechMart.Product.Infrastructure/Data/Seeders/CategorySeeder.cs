using Microsoft.EntityFrameworkCore;
using TechMart.Product.Domain.Category;

namespace TechMart.Product.Infrastructure.Data.Seeders;

public static class CategorySeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        if (await context.Categories.AnyAsync())
        {
            return; // Data already seeded
        }

        var categories = new List<Category>();

        // Root categories
        var electronics = new Category("Electronics", "Electronic devices and components");
        var computers = new Category("Computers & Laptops", "Desktop computers, laptops and accessories");
        var smartphones = new Category("Smartphones & Tablets", "Mobile devices and tablets");
        var gaming = new Category("Gaming", "Gaming consoles, games and accessories");
        var accessories = new Category("Accessories", "Various electronic accessories");

        categories.AddRange([electronics, computers, smartphones, gaming, accessories]);

        // Save root categories first to get IDs
        context.Categories.AddRange(categories);
        await context.SaveChangesAsync();

        // Sub-categories for Computers
        var laptops = new Category("Laptops", "Portable computers", computers.Id);
        var desktops = new Category("Desktop Computers", "Desktop PC systems", computers.Id);
        var monitors = new Category("Monitors", "Computer displays", computers.Id);
        var keyboards = new Category("Keyboards & Mice", "Input devices", computers.Id);

        // Sub-categories for Smartphones
        var phones = new Category("Smartphones", "Mobile phones", smartphones.Id);
        var tablets = new Category("Tablets", "Tablet devices", smartphones.Id);
        var phoneAccessories = new Category("Phone Accessories", "Cases, chargers, etc.", smartphones.Id);

        // Sub-categories for Gaming
        var consoles = new Category("Gaming Consoles", "PlayStation, Xbox, Nintendo", gaming.Id);
        var games = new Category("Video Games", "Games for all platforms", gaming.Id);
        var gamingAccessories = new Category("Gaming Accessories", "Controllers, headsets, etc.", gaming.Id);

        // Sub-categories for Accessories
        var cables = new Category("Cables & Adapters", "Various cables and adapters", accessories.Id);
        var chargers = new Category("Chargers & Power", "Power supplies and chargers", accessories.Id);
        var storage = new Category("Storage", "External drives and memory", accessories.Id);

        var subCategories = new[]
        {
            laptops, desktops, monitors, keyboards,
            phones, tablets, phoneAccessories,
            consoles, games, gamingAccessories,
            cables, chargers, storage
        };

        context.Categories.AddRange(subCategories);
        await context.SaveChangesAsync();
    }
}