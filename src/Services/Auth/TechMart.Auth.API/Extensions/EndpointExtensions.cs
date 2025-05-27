using System.Reflection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TechMart.Auth.API.Endpoints;

namespace TechMart.Auth.API.Extensions;

/// <summary>
/// Extensions para registro y mapeo de endpoints
/// </summary>
public static class EndpointExtensions
{
    /// <summary>
    /// Registra todos los endpoints del ensamblado
    /// </summary>
    public static IServiceCollection AddEndpoints(
        this IServiceCollection services,
        Assembly assembly
    )
    {
        var serviceDescriptors = assembly
            .DefinedTypes.Where(type =>
                type is { IsAbstract: false, IsInterface: false }
                && type.IsAssignableTo(typeof(IEndpoint))
            )
            .Select(type => ServiceDescriptor.Transient(typeof(IEndpoint), type))
            .ToArray();

        services.TryAddEnumerable(serviceDescriptors);

        return services;
    }

    /// <summary>
    /// Mapea todos los endpoints registrados
    /// </summary>
    public static WebApplication MapEndpoints(this WebApplication app)
    {
        var endpoints = app.Services.GetRequiredService<IEnumerable<IEndpoint>>();

        // Crear grupo base para la API con versionado
        var apiGroup = app.MapGroup("api/v1").WithOpenApi();

        // Mapear cada endpoint
        foreach (var endpoint in endpoints)
        {
            endpoint.MapEndpoint(apiGroup);
        }

        return app;
    }
}
