using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TechMart.Product.Domain.Brand;
using TechMart.Product.Domain.Category;
using TechMart.Product.Domain.Inventory;
using TechMart.Product.Domain.Product;
using TechMart.Product.Infrastructure.Data.EntityFramework;
using TechMart.Product.Infrastructure.Data.EntityFramework.Interceptors;
using TechMart.Product.Infrastructure.Data.EntityFramework.UnitOfWork;
using TechMart.Product.Infrastructure.Identity;
using TechMart.Product.Infrastructure.Repositories.EntityFramework;
using TechMart.Product.Infrastructure.Services;
using TechMart.SharedKernel.Abstractions;

namespace TechMart.Product.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        // Database
        services.AddDatabase(configuration);
        
        // Repositories
        services.AddRepositories();
        
        // Services
        services.AddInfrastructureServices();
        
        // Unit of Work
        services.AddUnitOfWork();

        return services;
    }
    
    private static IServiceCollection AddDatabase(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
                               ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        // Register interceptors
        services.AddScoped<AuditableEntityInterceptor>();
        services.AddScoped<DomainEventInterceptor>();

        // Register DbContext with PostgreSQL
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorCodesToAdd: null);
                
                npgsqlOptions.CommandTimeout(30);
                npgsqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
            });

            options.EnableServiceProviderCaching();
            options.EnableSensitiveDataLogging(false);
        });

        return services;
    }
    
    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IBrandRepository, BrandRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IInventoryRepository, InventoryRepository>();

        return services;
    }
    
    private static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        // Core services
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IDateTimeService, DateTimeService>();
        
        // Add HttpContextAccessor for CurrentUserService
        services.AddHttpContextAccessor();

        return services;
    }
    
    private static IServiceCollection AddUnitOfWork(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        return services;
    }
    
    public static IServiceCollection AddInfrastructureDevelopment(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        services.AddInfrastructure(configuration);
        
        // Add development-specific services here
        // For example: in-memory caching, local file storage, etc.
        
        return services;
    }
    
    public static IServiceCollection AddInfrastructureProduction(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        services.AddInfrastructure(configuration);
        
        // Add production-specific services here
        // For example: Redis caching, cloud storage, monitoring, etc.
        
        return services;
    }
}