using TechMart.Product.Application;
using TechMart.Product.Infrastructure;
using TechMart.Product.Infrastructure.Data;
using TechMart.Product.Infrastructure.Data.Seeders;
using TechMart.Product.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddProductApiServices(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Configure pipeline
app.ConfigureProductApiPipeline();

// Seed database in development
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    try
    {
        await DatabaseSeeder.SeedAllAsync(context);
        app.Logger.LogInformation("Database seeded successfully");
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "An error occurred while seeding the database");
    }
}

app.Run();