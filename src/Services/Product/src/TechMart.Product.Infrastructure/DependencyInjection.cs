using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TechMart.Product.Application.Contracts.Caching;
using TechMart.Product.Domain.Brand;
using TechMart.Product.Domain.Category;
using TechMart.Product.Domain.Inventory;
using TechMart.Product.Domain.Product;
using TechMart.Product.Infrastructure.Caching;
using TechMart.Product.Infrastructure.Data.EntityFramework;
using TechMart.Product.Infrastructure.Data.EntityFramework.Interceptors;
using TechMart.Product.Infrastructure.Data.EntityFramework.UnitOfWork;
using TechMart.Product.Infrastructure.Identity;
using TechMart.Product.Infrastructure.Repositories.EntityFramework;
using TechMart.Product.Infrastructure.Services;
using TechMart.SharedKernel.Abstractions;
using TechMart.SharedKernel.Extensions.ServiceCollection;
using IUnitOfWork = TechMart.Product.Domain.Abstractions.IUnitOfWork;

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
        
        // Add interceptors
        services.AddInterceptors();
        
        // Unit of Work
        services.AddUnitOfWork();

        return services;
    }
    
    private static IServiceCollection AddDatabase(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        services.AddTechMartPostgreSQL<ApplicationDbContext>(configuration);
        
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
    
    private static void AddCacheServices(IServiceCollection services, IConfiguration configuration)
    {
        // Try to configure Redis first
        var redisConnectionString = configuration.GetConnectionString("Redis");
        
        if (!string.IsNullOrEmpty(redisConnectionString))
        {
            try
            {
                // Use generic Redis configuration from SharedKernel
                services.AddTechMartRedis(configuration);
                
                // Register the specific cache service implementation for this microservice
                services.AddScoped<ICacheService, RedisCacheService>();
            }
            catch (Exception)
            {
                // If Redis configuration fails, fallback to in-memory cache
                AddInMemoryCacheServices(services);
            }
        }
        else
        {
            // No Redis configured, use in-memory cache
            AddInMemoryCacheServices(services);
        }
    }
    
    private static void AddInMemoryCacheServices(IServiceCollection services)
    {
        // Use generic in-memory cache configuration from SharedKernel
        services.AddTechMartInMemoryCache();
        
        // Register the specific cache service implementation for this microservice
        services.AddScoped<ICacheService, InMemoryCacheService>();
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
    
    private static IServiceCollection AddInterceptors(this IServiceCollection services)
    {
        services.AddScoped<AuditableEntityInterceptor>();
        services.AddScoped<DomainEventInterceptor>();

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