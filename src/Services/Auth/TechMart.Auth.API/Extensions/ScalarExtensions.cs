using Scalar.AspNetCore;

namespace TechMart.Auth.API.Extensions;

/// <summary>
/// Extensions para configuración de Scalar (alternativa moderna a Swagger UI)
/// </summary>
public static class ScalarExtensions
{
    /// <summary>
    /// Configura Scalar como alternativa moderna a Swagger UI
    /// </summary>
    public static WebApplication UseScalarDocumentation(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment() && !app.Environment.IsStaging())
            return app;

        // Configurar Scalar con las opciones correctas
        app.MapScalarApiReference(options =>
        {
            // Configuración básica
            options.Title = "TechMart Auth API - {documentName}";
            options.Theme = ScalarTheme.Purple;
            options.Layout = ScalarLayout.Modern;
            options.OpenApiRoutePattern = "/swagger/{documentName}/swagger.json";

            // Configuración de UI
            options.ShowSidebar = true;
            options.DarkMode = false;
            options.HideDownloadButton = false;
            options.HideTestRequestButton = false;
            options.DefaultOpenAllTags = false;
            options.SearchHotKey = "k";

            // Cliente HTTP por defecto
            options.DefaultHttpClient = new KeyValuePair<ScalarTarget, ScalarClient>(
                ScalarTarget.CSharp,
                ScalarClient.HttpClient
            );

            // Configuración de autenticación
            options.Authentication = new ScalarAuthenticationOptions
            {
                PreferredSecurityScheme = "Bearer",
            };

            // Metadatos adicionales
            options.Metadata = new Dictionary<string, string>
            {
                ["version"] = "1.0.0",
                ["environment"] = app.Environment.EnvironmentName,
                ["framework"] = ".NET 9",
            };

            // CSS personalizado
            options.CustomCss = """
            :root {
                --scalar-color-1: #667eea;
                --scalar-color-2: #764ba2;
                --scalar-color-accent: #667eea;
            }
            .scalar-api-reference {
                --scalar-font: 'Inter', system-ui, sans-serif;
            }
            """;

            // Contenido adicional en el header
            options.HeaderContent = """
            <div style="background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); 
                       color: white; padding: 1rem; text-align: center; margin-bottom: 1rem;">
                <h2 style="margin: 0; font-weight: 700;">🚀 TechMart Auth API</h2>
                <p style="margin: 0.5rem 0 0 0; opacity: 0.9;">
                    Secure authentication and user management for TechMart e-commerce platform
                </p>
            </div>
            """;
        });

        return app;
    }

    /// <summary>
    /// Configuración simplificada de Scalar
    /// </summary>
    public static WebApplication UseScalarDocumentationSimple(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment() && !app.Environment.IsStaging())
            return app;

        // Configuración básica y limpia
        app.MapScalarApiReference(options =>
        {
            options.Title = "TechMart Auth API";
            options.Theme = ScalarTheme.Purple;
            options.OpenApiRoutePattern = "/swagger/{documentName}/swagger.json";
            options.DefaultHttpClient = new KeyValuePair<ScalarTarget, ScalarClient>(
                ScalarTarget.CSharp,
                ScalarClient.HttpClient
            );
        });

        return app;
    }
}
