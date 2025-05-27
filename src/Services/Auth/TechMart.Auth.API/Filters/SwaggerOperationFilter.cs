using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using TechMart.Auth.API.Common.Responses;

namespace TechMart.Auth.API.Filters;

/// <summary>
/// Filtro para personalizar operaciones individuales en Swagger
/// </summary>
public class SwaggerOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        // Agregar ejemplos de respuesta estándar
        AddStandardResponses(operation);

        // Personalizar summary y description si están vacíos
        if (string.IsNullOrEmpty(operation.Summary))
        {
            operation.Summary = GenerateSummaryFromPath(context.ApiDescription.RelativePath);
        }

        // Agregar información de seguridad
        if (
            context.ApiDescription.ActionDescriptor.EndpointMetadata.Any(m =>
                m.GetType().Name == "AuthorizeAttribute"
            )
        )
        {
            operation.Security = new List<OpenApiSecurityRequirement>
            {
                new()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer",
                            },
                        },
                        Array.Empty<string>()
                    },
                },
            };
        }
    }

    private static void AddStandardResponses(OpenApiOperation operation)
    {
        // Respuesta de éxito estándar
        if (!operation.Responses.ContainsKey("200"))
        {
            operation.Responses.Add(
                "200",
                new OpenApiResponse
                {
                    Description = "Success",
                    Content = new Dictionary<string, OpenApiMediaType>
                    {
                        ["application/json"] = new()
                        {
                            Schema = new OpenApiSchema
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.Schema,
                                    Id = nameof(ApiResponse),
                                },
                            },
                        },
                    },
                }
            );
        }

        // Respuesta de error de validación
        if (!operation.Responses.ContainsKey("400"))
        {
            operation.Responses.Add(
                "400",
                new OpenApiResponse
                {
                    Description = "Bad Request - Validation Error",
                    Content = new Dictionary<string, OpenApiMediaType>
                    {
                        ["application/json"] = new()
                        {
                            Schema = new OpenApiSchema
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.Schema,
                                    Id = nameof(ApiResponse),
                                },
                            },
                        },
                    },
                }
            );
        }

        // Respuesta de error de autorización
        if (!operation.Responses.ContainsKey("401"))
        {
            operation.Responses.Add(
                "401",
                new OpenApiResponse { Description = "Unauthorized - Authentication Required" }
            );
        }
    }

    private static string GenerateSummaryFromPath(string? path)
    {
        if (string.IsNullOrEmpty(path))
            return "API Operation";

        var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
        var lastSegment = segments.LastOrDefault() ?? "operation";

        return $"{char.ToUpper(lastSegment[0])}{lastSegment[1..]} operation";
    }
}
