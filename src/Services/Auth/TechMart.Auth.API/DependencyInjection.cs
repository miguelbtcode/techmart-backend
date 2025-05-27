using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using TechMart.Auth.API.Extensions;

namespace TechMart.Auth.API;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        // Endpoints
        services.AddEndpoints(Assembly.GetExecutingAssembly());

        // Problem details para manejo de errores
        services.AddProblemDetails();

        // JSON options
        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        return services;
    }
}
