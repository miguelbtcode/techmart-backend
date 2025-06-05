using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace TechMart.SharedKernel.Extensions.ServiceCollection;

/// <summary>
/// Extension methods for configuring CORS.
/// </summary>
public static class CorsExtensions
{
    /// <summary>
    /// Adds CORS with TechMart development configuration.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="policyName">The CORS policy name.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddTechMartCorsDevelopment(
        this IServiceCollection services,
        string policyName = "TechMartCorsPolicy")
    {
        services.AddCors(options =>
        {
            options.AddPolicy(policyName, builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            });
        });

        return services;
    }

    /// <summary>
    /// Adds CORS with TechMart production configuration.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="allowedOrigins">The allowed origins.</param>
    /// <param name="policyName">The CORS policy name.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddTechMartCorsProduction(
        this IServiceCollection services,
        string[] allowedOrigins,
        string policyName = "TechMartCorsPolicy")
    {
        services.AddCors(options =>
        {
            options.AddPolicy(policyName, builder =>
            {
                builder.WithOrigins(allowedOrigins)
                       .AllowAnyMethod()
                       .AllowAnyHeader()
                       .AllowCredentials();
            });
        });

        return services;
    }

    /// <summary>
    /// Adds CORS with custom configuration.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configureCors">Action to configure CORS options.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddTechMartCors(
        this IServiceCollection services,
        Action<CorsOptions> configureCors)
    {
        services.AddCors(configureCors);
        return services;
    }
}