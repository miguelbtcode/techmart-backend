using Asp.Versioning;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using TechMart.Product.Application.Contracts.Caching;
using TechMart.Product.Application.Contracts.Identity;
using TechMart.Product.Infrastructure.Caching;
using TechMart.Product.Infrastructure.Identity;
using TechMart.SharedKernel.Extensions.ApplicationBuilder;

namespace TechMart.Product.Api.Extensions;

public static class StartupExtensions
{
    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Add controllers with custom JSON options
        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.WriteIndented = false;
                options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
            });

        // Add problem details for better error responses
        services.AddProblemDetails();

        // Add health checks
        services.AddHealthChecks()
            .AddDbContextCheck<Infrastructure.Data.EntityFramework.ApplicationDbContext>();

        // Add API versioning
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ApiVersionReader = ApiVersionReader.Combine(
                new QueryStringApiVersionReader("version"),
                new HeaderApiVersionReader("X-Version"));
        }).AddApiExplorer(setup =>
        {
            setup.GroupNameFormat = "'v'VVV";
            setup.SubstituteApiVersionInUrl = true;
        });

        // Add rate limiting (if needed)
        services.AddRateLimiter(options =>
        {
            options.AddFixedWindowLimiter("ApiRateLimit", configure =>
            {
                configure.PermitLimit = 100;
                configure.Window = TimeSpan.FromMinutes(1);
                configure.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
                configure.QueueLimit = 5;
            });
        });

        return services;
    }

    public static IServiceCollection AddCaching(this IServiceCollection services, IConfiguration configuration)
    {
        var redisConnectionString = configuration.GetConnectionString("Redis");
        
        if (!string.IsNullOrEmpty(redisConnectionString))
        {
            // Configure Redis
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnectionString;
                options.InstanceName = "TechMartProduct";
            });
            
            services.AddScoped<ICacheService, RedisCacheService>();
        }
        else
        {
            // Fallback to in-memory cache
            services.AddMemoryCache();
            services.AddScoped<ICacheService, InMemoryCacheService>();
        }

        return services;
    }

    public static IServiceCollection AddIdentityServices(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<IClaimsService, ClaimsService>();
        
        return services;
    }

    public static WebApplication ConfigureMiddleware(this WebApplication app)
    {
        // Configure the HTTP request pipeline
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseTechMartSwagger();
            app.UseTechMartDetailedRequestLogging();
        }
        else
        {
            app.UseTechMartExceptionHandling();
            app.UseTechMartRequestLogging();
            app.UseHsts(); // HTTP Strict Transport Security
        }

        // Performance and monitoring
        app.UseTechMartPerformanceLogging();
        app.UseTechMartCorrelationId();

        // Security
        app.UseHttpsRedirection();
        app.UseCors("TechMartCorsPolicy");

        // Rate limiting
        app.UseRateLimiter();

        // Authentication & Authorization
        app.UseAuthentication();
        app.UseAuthorization();

        // Health checks
        app.MapHealthChecks("/health");
        app.MapHealthChecks("/health/ready");
        app.MapHealthChecks("/health/live");

        return app;
    }
}