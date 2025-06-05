using System.Diagnostics;
using System.Threading.RateLimiting;
using Asp.Versioning;
using Microsoft.AspNetCore.RateLimiting;
using TechMart.Product.Infrastructure.Data.EntityFramework;

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

        // Add health checks
        services.AddHealthChecks()
            .AddDbContextCheck<ApplicationDbContext>("database")
            .AddCheck("self", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy());

        // Add rate limiting
        services.AddRateLimiting(configuration);

        return services;
    }

    public static IServiceCollection AddRateLimiting(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddRateLimiter(options =>
        {
            // Global API rate limit
            options.AddFixedWindowLimiter("ApiRateLimit", configure =>
            {
                configure.PermitLimit = configuration.GetValue<int>("RateLimit:PermitLimit", 100);
                configure.Window = TimeSpan.FromMinutes(configuration.GetValue<int>("RateLimit:WindowMinutes", 1));
                configure.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                configure.QueueLimit = configuration.GetValue<int>("RateLimit:QueueLimit", 5);
            });

            // Search API specific rate limit (more restrictive)
            options.AddFixedWindowLimiter("SearchRateLimit", configure =>
            {
                configure.PermitLimit = configuration.GetValue<int>("RateLimit:SearchPermitLimit", 20);
                configure.Window = TimeSpan.FromMinutes(1);
                configure.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                configure.QueueLimit = 2;
            });

            // Admin operations rate limit
            options.AddFixedWindowLimiter("AdminRateLimit", configure =>
            {
                configure.PermitLimit = configuration.GetValue<int>("RateLimit:AdminPermitLimit", 50);
                configure.Window = TimeSpan.FromMinutes(1);
                configure.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                configure.QueueLimit = 10;
            });

            options.OnRejected = async (context, token) =>
            {
                context.HttpContext.Response.StatusCode = 429;
                if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                {
                    await context.HttpContext.Response.WriteAsync(
                        $"Too many requests. Please try again after {retryAfter.TotalMinutes} minute(s).", 
                        cancellationToken: token);
                }
                else
                {
                    await context.HttpContext.Response.WriteAsync(
                        "Too many requests. Please try again later.", 
                        cancellationToken: token);
                }
            };
        });

        return services;
    }

    public static WebApplication ConfigureMiddleware(this WebApplication app)
    {
        // Configure the HTTP request pipeline
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "TechMart API v1");
                c.RoutePrefix = "swagger";
                c.DisplayRequestDuration();
                c.EnableDeepLinking();
                c.EnableFilter();
                c.ShowExtensions();
                c.EnableValidator();
                c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
            });
            
            app.Use(async (context, next) =>
            {
                var loggerFactory = context.RequestServices.GetRequiredService<ILoggerFactory>();
                var logger = loggerFactory.CreateLogger("TechMart.DetailedRequestLogging");
                var stopwatch = Stopwatch.StartNew();
                var traceId = context.TraceIdentifier;

                // Log request details
                logger.LogInformation(
                    "Request started: {Method} {Path} - Headers: {@Headers} - TraceId: {TraceId}",
                    context.Request.Method,
                    context.Request.Path,
                    context.Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()),
                    traceId);

                try
                {
                    await next();
                }
                finally
                {
                    stopwatch.Stop();
                
                    logger.LogInformation(
                        "Request completed: {Method} {Path} - Status: {StatusCode} - Duration: {ElapsedMs}ms - TraceId: {TraceId}",
                        context.Request.Method,
                        context.Request.Path,
                        context.Response.StatusCode,
                        stopwatch.ElapsedMilliseconds,
                        traceId);
                }
            });
        }
        else
        {
            app.Use(async (context, next) =>
            {
                var loggerFactory = context.RequestServices.GetRequiredService<ILoggerFactory>();
                var logger = loggerFactory.CreateLogger("TechMart.DetailedRequestLogging");
                var stopwatch = Stopwatch.StartNew();
                var traceId = context.TraceIdentifier;

                // Log request details
                logger.LogInformation(
                    "Request started: {Method} {Path} - Headers: {@Headers} - TraceId: {TraceId}",
                    context.Request.Method,
                    context.Request.Path,
                    context.Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()),
                    traceId);

                try
                {
                    await next();
                }
                finally
                {
                    stopwatch.Stop();
                
                    logger.LogInformation(
                        "Request completed: {Method} {Path} - Status: {StatusCode} - Duration: {ElapsedMs}ms - TraceId: {TraceId}",
                        context.Request.Method,
                        context.Request.Path,
                        context.Response.StatusCode,
                        stopwatch.ElapsedMilliseconds,
                        traceId);
                }
            });
            
            app.UseHsts();
        }

        // Performance and monitoring
        app.Use(async (context, next) =>
        {
            var correlationId = context.Request.Headers["X-Correlation-ID"].FirstOrDefault() ?? Guid.NewGuid().ToString();
            
            context.Items["CorrelationId"] = correlationId;
            context.Response.Headers["X-Correlation-ID"] = correlationId;

            await next();
        });

        // Security
        app.UseHttpsRedirection();
        
        // CORS - must be before authentication
        app.UseCors("AllowDevOrigins");

        // Rate limiting - before authentication
        app.UseRateLimiter();

        // Authentication & Authorization
        app.UseAuthentication();
        app.UseAuthorization();

        // Health checks
        app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
        {
            ResponseWriter = async (context, report) =>
            {
                context.Response.ContentType = "application/json";
                var response = new
                {
                    status = report.Status.ToString(),
                    checks = report.Entries.Select(x => new
                    {
                        name = x.Key,
                        status = x.Value.Status.ToString(),
                        exception = x.Value.Exception?.Message,
                        duration = x.Value.Duration.ToString()
                    }),
                    totalDuration = report.TotalDuration.ToString()
                };
                await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response));
            }
        });

        app.MapHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("ready")
        });

        app.MapHealthChecks("/health/live", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
        {
            Predicate = _ => false
        });

        return app;
    }

    public static IServiceCollection AddProductApiConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        // Configure specific options for Product API
        services.Configure<CacheOptions>(configuration.GetSection("Cache"));
        services.Configure<RateLimitOptions>(configuration.GetSection("RateLimit"));
        services.Configure<FeatureOptions>(configuration.GetSection("Features"));

        return services;
    }
}

// Configuration classes
public class CacheOptions
{
    public int DefaultExpirationMinutes { get; set; } = 30;
    public int ProductCacheExpirationMinutes { get; set; } = 60;
    public int CategoryCacheExpirationMinutes { get; set; } = 120;
    public int BrandCacheExpirationMinutes { get; set; } = 120;
}

public class RateLimitOptions
{
    public int PermitLimit { get; set; } = 100;
    public int WindowMinutes { get; set; } = 1;
    public int QueueLimit { get; set; } = 5;
    public int SearchPermitLimit { get; set; } = 20;
    public int AdminPermitLimit { get; set; } = 50;
}

public class FeatureOptions
{
    public bool EnableCaching { get; set; } = true;
    public bool EnableDetailedLogging { get; set; } = false;
    public bool EnableMetrics { get; set; } = true;
    public bool EnableRateLimiting { get; set; } = true;
}