using TechMart.Auth.Api.Extensions;
using TechMart.Auth.Api.Middlewares;
using TechMart.Auth.Infrastructure.Data;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Serilog Configuration
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .WriteTo.Console()
    .WriteTo.File(
        "logs/AuthMicroservice-api-.log",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 7
    )
    .CreateLogger();

try
{
    Log.Information("🚀 Starting AuthMicroservice API...");

    builder.Host.UseSerilog();

    // Add all services (Infrastructure + Application + API)
    builder.Services.AddApiServices(builder.Configuration);

    var app = builder.Build();

    // Configure the HTTP request pipeline
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "AuthMicroservice API V1");
            c.RoutePrefix = string.Empty; // Set Swagger UI at app root
            c.DisplayRequestDuration();
            c.EnableDeepLinking();
        });
    }

    // Middleware pipeline
    app.UseMiddleware<GlobalExceptionMiddleware>();
    app.UseHttpsRedirection();

    // Use environment-specific CORS policy
    var corsPolicy = app.Environment.IsDevelopment() ? "Development" : "Production";
    app.UseCors(corsPolicy);

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    // Health checks endpoints
    app.MapHealthChecks("/health");
    app.MapHealthChecks(
        "/health/ready",
        new HealthCheckOptions { Predicate = check => check.Tags.Contains("ready") }
    );
    app.MapHealthChecks("/health/live", new HealthCheckOptions { Predicate = _ => false });

    // Database initialization in development
    // if (app.Environment.IsDevelopment())
    // {
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<AuthDbContext>();

    Log.Information("🗄️ Ensuring database is created...");
    await context.Database.EnsureCreatedAsync();
    Log.Information("✅ Database initialization completed");
    // }

    Log.Information("🎯 AuthMicroservice API is ready!");
    Log.Information("📋 Available endpoints:");
    Log.Information("   • API: /api/v1/*");
    Log.Information("   • Swagger: / (Development only)");
    Log.Information("   • Health: /health, /health/ready, /health/live");

    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "❌ Application terminated unexpectedly: {ErrorMessage}", ex.Message);

    // Logging más detallado para debugging
    if (ex.InnerException != null)
    {
        Log.Fatal(
            ex.InnerException,
            "💥 Inner exception: {InnerMessage}",
            ex.InnerException.Message
        );
    }

    throw;
}
finally
{
    Log.Information("🛑 Shutting down AuthMicroservice API...");
    Log.CloseAndFlush();
}
