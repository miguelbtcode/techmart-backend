using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using TechMart.Auth.API.Common.Responses;
using TechMart.Auth.Infrastructure.Settings;

namespace TechMart.Auth.API.Extensions;

/// <summary>
/// Extensions para configuración de servicios
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Configura la autenticación JWT
    /// </summary>
    public static IServiceCollection ConfigureAuthentication(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var jwtSettings =
            configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>()
            ?? throw new InvalidOperationException("JWT settings are not configured");

        jwtSettings.Validate();

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = jwtSettings.ValidateIssuer,
                    ValidateAudience = jwtSettings.ValidateAudience,
                    ValidateLifetime = jwtSettings.ValidateLifetime,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSettings.SecretKey)
                    ),
                    ClockSkew = TimeSpan.FromSeconds(jwtSettings.ClockSkewSeconds),
                };

                // Configurar eventos para mejor logging
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        context.HttpContext.Items["AuthError"] = context.Exception.Message;
                        return Task.CompletedTask;
                    },
                    OnChallenge = context =>
                    {
                        context.HandleResponse();
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";

                        var response = ApiResponse.Failure("Authentication required");
                        return context.Response.WriteAsync(JsonSerializer.Serialize(response));
                    },
                };
            });

        services.AddAuthorization();

        return services;
    }

    /// <summary>
    /// Configura CORS
    /// </summary>
    public static IServiceCollection ConfigureCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(
                "AllowedOrigins",
                policy =>
                {
                    policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                }
            );
        });

        return services;
    }

    /// <summary>
    /// Configura health checks
    /// </summary>
    public static IServiceCollection ConfigureHealthChecks(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var healthChecksBuilder = services.AddHealthChecks();

        // Always add self check
        healthChecksBuilder.AddCheck(
            "self",
            () =>
                Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy(
                    "API is running"
                ),
            tags: ["ready"]
        );

        // Add database check if connection string exists
        var dbConnectionString = configuration.GetConnectionString("AuthDatabase");
        if (!string.IsNullOrEmpty(dbConnectionString))
        {
            healthChecksBuilder.AddSqlServer(
                dbConnectionString,
                name: "sql-server",
                tags: ["ready"]
            );
        }

        // Add Redis check if connection string exists
        var redisConnectionString = configuration.GetConnectionString("Redis");
        if (!string.IsNullOrEmpty(redisConnectionString))
        {
            try
            {
                healthChecksBuilder.AddRedis(redisConnectionString, name: "redis", tags: ["ready"]);
            }
            catch (Exception)
            {
                // Redis health check failed to configure, skip it
                Console.WriteLine("⚠️ Warning: Redis health check could not be configured");
            }
        }

        return services;
    }
}
