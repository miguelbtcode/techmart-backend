using Microsoft.AspNetCore.Builder;

namespace TechMart.SharedKernel.Extensions.ApplicationBuilder;

/// <summary>
/// Extension methods for configuring Swagger UI.
/// </summary>
public static class SwaggerUIExtensions
{
    /// <summary>
    /// Adds Swagger UI with TechMart configuration.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <param name="routePrefix">The route prefix for Swagger UI.</param>
    /// <returns>The application builder.</returns>
    public static IApplicationBuilder UseTechMartSwagger(this IApplicationBuilder app, string routePrefix = "swagger")
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "TechMart API v1");
            c.RoutePrefix = routePrefix;
            c.DisplayRequestDuration();
            c.EnableDeepLinking();
            c.EnableFilter();
            c.ShowExtensions();
            c.EnableValidator();
            c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
        });

        return app;
    }

    /// <summary>
    /// Adds Swagger UI with multiple API versions.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <param name="apiVersions">The API versions to display.</param>
    /// <param name="routePrefix">The route prefix for Swagger UI.</param>
    /// <returns>The application builder.</returns>
    public static IApplicationBuilder UseTechMartSwaggerVersioning(
        this IApplicationBuilder app,
        string[] apiVersions,
        string routePrefix = "swagger")
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            foreach (var version in apiVersions)
            {
                c.SwaggerEndpoint($"/swagger/{version}/swagger.json", $"TechMart API {version}");
            }
            
            c.RoutePrefix = routePrefix;
            c.DisplayRequestDuration();
            c.EnableDeepLinking();
            c.EnableFilter();
        });

        return app;
    }

    /// <summary>
    /// Adds Swagger UI for development environment only.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <param name="isDevelopment">Whether the environment is development.</param>
    /// <returns>The application builder.</returns>
    public static IApplicationBuilder UseTechMartSwaggerDevelopment(this IApplicationBuilder app, bool isDevelopment)
    {
        if (isDevelopment)
        {
            app.UseTechMartSwagger();
        }

        return app;
    }
}