using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace TechMart.Auth.API.Filters;

/// <summary>
/// Filtro para personalizar el documento Swagger
/// </summary>
public class SwaggerDocumentFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        // Agregar información adicional al documento
        if (!swaggerDoc.Info.Extensions.ContainsKey("x-logo"))
        {
            swaggerDoc.Info.Extensions.Add(
                "x-logo",
                new Microsoft.OpenApi.Any.OpenApiObject
                {
                    ["url"] = new Microsoft.OpenApi.Any.OpenApiString(
                        "https://techmart.com/logo.png"
                    ),
                    ["altText"] = new Microsoft.OpenApi.Any.OpenApiString("TechMart Logo"),
                }
            );
        }

        // Agregar servers
        swaggerDoc.Servers = new List<OpenApiServer>
        {
            new() { Url = "http://localhost:5254", Description = "Development Server" },
            new() { Url = "https://api.techmart.com", Description = "Production Server" },
        };

        // Personalizar tags
        swaggerDoc.Tags = new List<OpenApiTag>
        {
            new()
            {
                Name = "Authentication",
                Description = "Endpoints for user authentication and authorization",
                ExternalDocs = new OpenApiExternalDocs
                {
                    Description = "Authentication Guide",
                    Url = new Uri("https://docs.techmart.com/auth"),
                },
            },
            new()
            {
                Name = "Users",
                Description = "User management and profile operations",
                ExternalDocs = new OpenApiExternalDocs
                {
                    Description = "User Management Guide",
                    Url = new Uri("https://docs.techmart.com/users"),
                },
            },
            new() { Name = "Health", Description = "API health and monitoring endpoints" },
        };
    }
}
