using Serilog;
using TechMart.Auth.API.Middleware;
using TechMart.Auth.Infrastructure;

namespace TechMart.Auth.API.Extensions;

/// <summary>
/// Extensions para configuración de la aplicación
/// </summary>
public static class WebApplicationExtensions
{
    /// <summary>
    /// Configura el pipeline de middleware
    /// </summary>
    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        // Logging de requests
        app.UseSerilogRequestLogging(options =>
        {
            options.MessageTemplate =
                "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
            options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
            {
                diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
                diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
                diagnosticContext.Set(
                    "UserAgent",
                    httpContext.Request.Headers.UserAgent.FirstOrDefault()
                );
                diagnosticContext.Set("TraceId", httpContext.TraceIdentifier);
            };
        });

        // Exception handling
        app.UseMiddleware<GlobalExceptionMiddleware>();

        // Swagger en desarrollo
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "TechMart Auth API v1");
                options.RoutePrefix = "swagger";
                options.DisplayRequestDuration();
                options.EnableTryItOutByDefault();
            });
        }

        // Security headers
        app.UseSecurityHeaders();

        // HTTPS redirection
        // app.UseHttpsRedirection();

        if (!app.Environment.IsDevelopment())
        {
            app.UseHttpsRedirection();
        }

        // CORS
        app.UseCors("AllowedOrigins");

        // Authentication & Authorization
        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }

    /// <summary>
    /// Configura headers de seguridad
    /// </summary>
    public static WebApplication UseSecurityHeaders(this WebApplication app)
    {
        app.Use(
            async (context, next) =>
            {
                // Remover headers que revelan información del servidor
                context.Response.Headers.Remove("Server");
                context.Response.Headers.Remove("X-Powered-By");

                // Agregar headers de seguridad
                context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
                context.Response.Headers.Append("X-Frame-Options", "DENY");
                context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
                context.Response.Headers.Append(
                    "Referrer-Policy",
                    "strict-origin-when-cross-origin"
                );

                if (context.Request.IsHttps)
                {
                    context.Response.Headers.Append(
                        "Strict-Transport-Security",
                        "max-age=31536000; includeSubDomains"
                    );
                }

                await next();
            }
        );

        return app;
    }

    /// <summary>
    /// Inicializa la base de datos
    /// </summary>
    public static async Task<WebApplication> InitializeDatabaseAsync(this WebApplication app)
    {
        try
        {
            Log.Information("Initializing database...");

            await app.Services.MigrateDatabaseAsync();
            await app.Services.SeedDataAsync();

            Log.Information("Database initialization completed successfully");
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Database initialization failed");
            throw;
        }

        return app;
    }

    /// <summary>
    /// Configura Serilog
    /// </summary>
    public static ConfigureHostBuilder ConfigureSerilog(this ConfigureHostBuilder host)
    {
        host.UseSerilog(
            (context, configuration) =>
            {
                configuration
                    .ReadFrom.Configuration(context.Configuration)
                    .Enrich.FromLogContext()
                    .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
                    .Enrich.WithProperty("Application", "TechMart.Auth.API")
                    .WriteTo.Console(
                        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}"
                    )
                    .WriteTo.File(
                        path: "logs/techmart-auth-.log",
                        rollingInterval: RollingInterval.Day,
                        retainedFileCountLimit: 7,
                        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}"
                    );
            }
        );

        return host;
    }
}
