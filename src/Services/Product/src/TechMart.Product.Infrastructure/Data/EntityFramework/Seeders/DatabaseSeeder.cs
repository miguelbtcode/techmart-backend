namespace TechMart.Product.Infrastructure.Data.EntityFramework.Seeders;

public static class DatabaseSeeder
{
    public static async Task SeedAllAsync(ApplicationDbContext context)
    {
        try
        {
            // Ensure database is created
            await context.Database.EnsureCreatedAsync();

            // Seed data in order (due to foreign key dependencies)
            await BrandSeeder.SeedAsync(context);
            await CategorySeeder.SeedAsync(context);
            await ProductSeeder.SeedAsync(context);

            // Create some inventory records for the products
            await InventorySeeder.SeedAsync(context);
        }
        catch (Exception ex)
        {
            // Log the exception (you might want to use ILogger here)
            throw new InvalidOperationException("An error occurred while seeding the database.", ex);
        }
    }
}