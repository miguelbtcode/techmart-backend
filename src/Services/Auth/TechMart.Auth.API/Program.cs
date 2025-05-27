using System.Text.Json;
using Serilog;
using TechMart.Auth.Application;
using TechMart.Auth.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Logging configuration
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder
    .Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System
            .Text
            .Json
            .Serialization
            .JsonIgnoreCondition
            .WhenWritingNull;
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc(
        "v1",
        new()
        {
            Title = "TechMart Auth API",
            Version = "v1",
            Description =
                "Authentication and authorization service for TechMart e-commerce platform",
        }
    );

    // Add JWT authentication to Swagger
    c.AddSecurityDefinition(
        "Bearer",
        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
        {
            Description =
                "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
            Name = "Authorization",
            In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
            Scheme = "Bearer",
        }
    );

    c.AddSecurityRequirement(
        new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
        {
            {
                new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Reference = new Microsoft.OpenApi.Models.OpenApiReference
                    {
                        Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                        Id = "Bearer",
                    },
                },
                Array.Empty<string>()
            },
        }
    );
});

// CORS configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowedOrigins",
        policy =>
        {
            policy
                .WithOrigins(
                    "http://localhost:3000", // React dev server
                    "https://techmart.com",
                    "https://www.techmart.com"
                )
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        }
    );
});

// Add health checks
builder
    .Services.AddHealthChecks()
    .AddSqlServer(builder.Configuration.GetConnectionString("AuthDatabase")!)
    .AddRedis(builder.Configuration.GetConnectionString("Redis")!);

// Application layers
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "TechMart Auth API V1");
        c.RoutePrefix = string.Empty; // Swagger at root
    });
}

// Security headers
app.Use(
    async (context, next) =>
    {
        context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
        context.Response.Headers.Add("X-Frame-Options", "DENY");
        context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
        context.Response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");
        await next();
    }
);

app.UseHttpsRedirection();
app.UseCors("AllowedOrigins");

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// Request logging
app.UseSerilogRequestLogging();

// Health checks
app.MapHealthChecks("/health");

// Controllers
app.MapControllers();

// Global exception handling
app.UseExceptionHandler("/error");

// Database migration and seeding
try
{
    Log.Information("Starting TechMart Auth API...");

    // Migrate database and seed initial data
    await app.Services.MigrateDatabaseAsync();
    await app.Services.SeedDataAsync();

    Log.Information("Database migration and seeding completed successfully");
}
catch (Exception ex)
{
    Log.Fatal(ex, "Failed to start TechMart Auth API");
    throw;
}

try
{
    Log.Information("TechMart Auth API started successfully");
    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "TechMart Auth API terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
