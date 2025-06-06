using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using TechMart.Product.Application.Contracts.Identity;
using TechMart.Product.Application.Contracts.Infrastructure;
using TechMart.Product.Domain.Brand;
using TechMart.Product.Domain.Category;
using TechMart.Product.Domain.Inventory;
using TechMart.Product.Domain.Product;
using TechMart.Product.Infrastructure.Caching;
using TechMart.Product.Infrastructure.Data;
using TechMart.Product.Infrastructure.Data.Interceptors;
using TechMart.Product.Infrastructure.Data.UnitOfWork;
using TechMart.Product.Infrastructure.Identity;
using TechMart.Product.Infrastructure.Repositories;
using TechMart.Product.Infrastructure.Services;
using TechMart.SharedKernel.Abstractions;
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
        
        // Cache
        services.AddCacheServices(configuration);
        
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
        var connectionString = configuration.GetConnectionString("PostgreSQL")
                               ?? throw new InvalidOperationException($"Connection string '{"PostgreSQL"}' not found.");

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorCodesToAdd: null);
                
                npgsqlOptions.CommandTimeout(30);
            });

            options.EnableServiceProviderCaching();
            options.EnableSensitiveDataLogging(false);
            options.UseSnakeCaseNamingConvention();
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
    
    private static void AddCacheServices(this IServiceCollection services, IConfiguration configuration)
    {
        var redisConnectionString = configuration.GetConnectionString("Redis");

        if (!string.IsNullOrWhiteSpace(redisConnectionString))
        {
            try
            {
                // Configurar y registrar ConnectionMultiplexer como singleton
                services.AddSingleton<IConnectionMultiplexer>(sp =>
                    ConnectionMultiplexer.Connect(redisConnectionString));

                // Registrar Redis como IDistributedCache
                services.AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = redisConnectionString;
                });

                // Registrar implementación concreta del servicio de caché
                services.AddScoped<ICacheService, RedisCacheService>();
            }
            catch (Exception)
            {
                AddInMemoryCacheServices(services);
            }
        }
        else
        {
            AddInMemoryCacheServices(services);
        }
    }
    
    private static void AddInMemoryCacheServices(IServiceCollection services)
    {
        // Use generic in-memory cache configuration from SharedKernel
        services.AddDistributedMemoryCache();
        
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
        
        // Add claims service
        services.AddScoped<IClaimsService, ClaimsService>();
        
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
}