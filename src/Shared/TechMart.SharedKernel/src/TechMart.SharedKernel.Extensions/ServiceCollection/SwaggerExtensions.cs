using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace TechMart.SharedKernel.Extensions.ServiceCollection;

/// <summary>
/// Extension methods for configuring Swagger/OpenAPI.
/// </summary>
public static class SwaggerExtensions
{
    /// <summary>
    /// Adds Swagger with TechMart standard configuration.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="title">The API title.</param>
    /// <param name="version">The API version.</param>
    /// <param name="description">The API description.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddTechMartSwagger(
        this IServiceCollection services,
        string title = "TechMart API",
        string version = "v1",
        string? description = null)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc(version, new OpenApiInfo
            {
                Title = title,
                Version = version,
                Description = description ?? $"{title} - Built with TechMart SharedKernel",
                Contact = new OpenApiContact
                {
                    Name = "TechMart Development Team",
                    Email = "dev@techmart.com"
                }
            });
            
            // Annotations
            c.UseInlineDefinitionsForEnums();
            c.UseOneOfForPolymorphism();
            c.UseAllOfForInheritance();
            c.SupportNonNullableReferenceTypes();

            // JWT Authentication
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    []
                }
            });

            // Include XML comments if available
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                c.IncludeXmlComments(xmlPath);
            }
        });

        return services;
    }
    
    /// <summary>
    /// Adds Swagger with multiple API versions.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="apiVersions">The API versions to document.</param>
    /// <param name="title">The API title.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddTechMartSwaggerVersioning(
        this IServiceCollection services,
        string[] apiVersions,
        string title = "TechMart API")
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            foreach (var version in apiVersions)
            {
                c.SwaggerDoc(version, new OpenApiInfo
                {
                    Title = title,
                    Version = version,
                    Description = $"{title} {version} - Built with TechMart SharedKernel"
                });
            }

            // Add common configuration
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme.",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    []
                }
            });
        });

        return services;
    }
}