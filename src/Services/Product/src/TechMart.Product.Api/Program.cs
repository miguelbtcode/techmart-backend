using TechMart.Product.Application;
using TechMart.Product.Infrastructure;
using TechMart.Product.Infrastructure.Data.EntityFramework;
using TechMart.Product.Infrastructure.Data.EntityFramework.Seeders;
using TechMart.SharedKernel.Extensions.ApplicationBuilder;
using TechMart.SharedKernel.Extensions.ServiceCollection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Add API Explorer and Swagger
builder.Services.AddTechMartSwagger("TechMart Product API", "v1", "Product management API for TechMart e-commerce platform");

// Add CORS
builder.Services.AddTechMartCorsDevelopment();

// Add authentication and authorization
if (builder.Configuration.GetSection("Jwt").Exists())
{
    builder.Services.AddTechMartAuthentication(builder.Configuration);
    builder.Services.AddTechMartAuthorization();
}

// Add Application layer
builder.Services.AddApplication();

// Add Infrastructure layer
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddInfrastructureDevelopment(builder.Configuration);
}
else
{
    builder.Services.AddInfrastructureProduction(builder.Configuration);
}

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseTechMartSwagger();
    app.UseTechMartDetailedRequestLogging();
}
else
{
    app.UseTechMartRequestLogging();
}

// Global exception handling
app.UseTechMartExceptionHandling(app.Environment.IsDevelopment());

// Add correlation ID
app.UseTechMartCorrelationId();

// Security headers and HTTPS
app.UseHttpsRedirection();

// CORS
app.UseCors("TechMartCorsPolicy");

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// Map controllers
app.MapControllers();

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