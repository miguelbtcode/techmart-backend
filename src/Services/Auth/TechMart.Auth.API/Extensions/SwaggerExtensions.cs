using System.Reflection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using TechMart.Auth.API.Filters;

namespace TechMart.Auth.API.Extensions;

/// <summary>
/// Extensions para configuración de Swagger y documentación de API
/// </summary>
public static class SwaggerExtensions
{
    /// <summary>
    /// Configura Swagger con autenticación JWT y documentación completa
    /// </summary>
    public static IServiceCollection ConfigureSwaggerUI(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            // Información básica de la API
            options.SwaggerDoc(
                "v1",
                new OpenApiInfo
                {
                    Title = "TechMart Auth API",
                    Version = "v1.0.0",
                    Description =
                        "Authentication and user management API for TechMart e-commerce platform",
                    Contact = new OpenApiContact
                    {
                        Name = "TechMart Development Team",
                        Email = "dev@techmart.com",
                        Url = new Uri("https://techmart.com"),
                    },
                    License = new OpenApiLicense
                    {
                        Name = "MIT License",
                        Url = new Uri("https://opensource.org/licenses/MIT"),
                    },
                    TermsOfService = new Uri("https://techmart.com/terms"),
                }
            );

            // Configurar autenticación JWT
            var securityScheme = new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Description =
                    "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Reference = new OpenApiReference
                {
                    Id = JwtBearerDefaults.AuthenticationScheme,
                    Type = ReferenceType.SecurityScheme,
                },
            };

            options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, securityScheme);

            // Requerir autenticación para endpoints protegidos
            options.AddSecurityRequirement(
                new OpenApiSecurityRequirement { { securityScheme, Array.Empty<string>() } }
            );

            // Personalizar esquemas
            options.CustomSchemaIds(type =>
            {
                var name = type.FullName?.Replace('+', '.');
                return name?.Contains("Endpoint") == true
                    ? name.Split('.').Last().Replace("Endpoint", "").Replace("Request", "")
                    : name;
            });

            // Incluir comentarios XML si existen
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                options.IncludeXmlComments(xmlPath);
            }

            // Configurar ejemplos de respuesta
            options.EnableAnnotations();

            // Agrupar por tags
            options.TagActionsBy(api =>
            {
                if (api.GroupName != null)
                    return [api.GroupName];

                var controllerName = api.ActionDescriptor.RouteValues["controller"];
                return [controllerName ?? "Default"];
            });

            // Configurar operaciones
            options.DocInclusionPredicate((name, api) => true);

            // Personalizar documentación
            options.DocumentFilter<SwaggerDocumentFilter>();
            options.OperationFilter<SwaggerOperationFilter>();
        });

        return services;
    }

    /// <summary>
    /// Configura el pipeline de Swagger con Swagger UI y Scalar
    /// </summary>
    public static WebApplication UseSwaggerDocumentation(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment() && !app.Environment.IsStaging())
            return app;

        app.UseSwagger(options =>
        {
            options.RouteTemplate = "swagger/{documentName}/swagger.json";
        });

        // Swagger UI tradicional
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "TechMart Auth API v1");
            options.RoutePrefix = "swagger-ui";

            // Configuraciones de UI
            options.DisplayRequestDuration();
            options.EnableTryItOutByDefault();
            options.EnableFilter();
            options.ShowExtensions();
            options.EnableValidator();

            // Personalización de tema
            options.InjectStylesheet("/swagger-ui/custom.css");
            options.InjectJavascript("/swagger-ui/custom.js");

            // OAuth2 si es necesario en el futuro
            options.OAuthClientId("swagger-ui");
            options.OAuthAppName("TechMart Auth API");
            options.OAuthUsePkce();
        });

        return app;
    }
}
