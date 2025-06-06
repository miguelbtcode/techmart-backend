using Microsoft.EntityFrameworkCore;
using TechMart.Product.Domain.Inventory;

namespace TechMart.Product.Infrastructure.Data.Seeders;

public static class InventorySeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        if (await context.Inventories.AnyAsync())
        {
            return; // Data already seeded
        }

        var products = await context.Products.ToListAsync();
        var inventories = new List<Inventory>();

        foreach (var product in products)
        {
            // Create inventory with random stock levels for demo
            var random = new Random();
            var initialStock = random.Next(10, 100);
            var reorderLevel = random.Next(5, 20);

            var inventory = new Inventory(product.Id, product.Sku.Value, initialStock);
            inventory.SetReorderLevel(reorderLevel);

            inventories.Add(inventory);
        }

        context.Inventories.AddRange(inventories);
        await context.SaveChangesAsync();
    }
}