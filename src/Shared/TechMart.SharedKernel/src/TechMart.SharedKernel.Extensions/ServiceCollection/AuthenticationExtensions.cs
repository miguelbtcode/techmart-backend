using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace TechMart.SharedKernel.Extensions.ServiceCollection;

/// <summary>
/// Extension methods for configuring authentication.
/// </summary>
public static class AuthenticationExtensions
{
    /// <summary>
    /// Adds JWT authentication with TechMart configuration.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <param name="jwtSectionName">The JWT configuration section name.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddTechMartAuthentication(
        this IServiceCollection services,
        IConfiguration configuration,
        string jwtSectionName = "Jwt")
    {
        var jwtSettings = configuration.GetSection(jwtSectionName);
        var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey is required");
        var issuer = jwtSettings["Issuer"] ?? throw new InvalidOperationException("JWT Issuer is required");
        var audience = jwtSettings["Audience"] ?? throw new InvalidOperationException("JWT Audience is required");

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.RequireHttpsMetadata = false; // Set to true in production
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = issuer,
                ValidAudience = audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                ClockSkew = TimeSpan.FromMinutes(5)
            };

            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                    {
                        context.Response.Headers.Append("Token-Expired", "true");
                    }
                    return Task.CompletedTask;
                },
                OnChallenge = context =>
                {
                    context.HandleResponse();
                    context.Response.StatusCode = 401;
                    context.Response.ContentType = "application/json";
                    var result = System.Text.Json.JsonSerializer.Serialize(new
                    {
                        success = false,
                        message = "You are not authorized to access this resource.",
                        errors = new[] { "Authentication required" }
                    });
                    return context.Response.WriteAsync(result);
                }
            };
        });

        return services;
    }

    /// <summary>
    /// Adds JWT authentication with custom token validation parameters.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configureOptions">Action to configure JWT options.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddTechMartAuthentication(
        this IServiceCollection services,
        Action<JwtBearerOptions> configureOptions)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(configureOptions);

        return services;
    }

    /// <summary>
    /// Adds authorization policies for TechMart.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddTechMartAuthorization(this IServiceCollection services)
    {
        services.AddAuthorizationBuilder()
            .AddPolicy("AdminOnly", policy =>
                policy.RequireRole("Admin"))
            .AddPolicy("UserOrAdmin", policy =>
                policy.RequireRole("User", "Admin"))
            .AddPolicy("RequireUserId", policy =>
                policy.RequireClaim("sub"))
            .AddPolicy("RequireEmail", policy =>
                policy.RequireClaim("email"));

        return services;
    }
}