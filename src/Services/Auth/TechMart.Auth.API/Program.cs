using System.Reflection;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using TechMart.Auth.API;
using TechMart.Auth.API.Extensions;
using TechMart.Auth.Application;
using TechMart.Auth.Infrastructure;
using TechMart.Auth.Infrastructure.Settings;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Logging configuration
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog(
    (context, loggerConfig) => loggerConfig.ReadFrom.Configuration(context.Configuration)
);

builder.Services.AddHealthChecks();

// Add services to the container
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.SerializerOptions.DefaultIgnoreCondition = System
        .Text
        .Json
        .Serialization
        .JsonIgnoreCondition
        .WhenWritingNull;
});

// JWT Configuration
var jwtSettings = builder.Configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>()!;
jwtSettings.Validate();

builder
    .Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = jwtSettings.ValidateIssuer,
            ValidateAudience = jwtSettings.ValidateAudience,
            ValidateLifetime = jwtSettings.ValidateLifetime,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings.SecretKey)
            ),
            ClockSkew = TimeSpan.FromSeconds(jwtSettings.ClockSkewSeconds),
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddSwaggerGenWithAuth();

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowedOrigins",
        policy =>
        {
            policy
                .WithOrigins(
                    "http://localhost:3000",
                    "http://localhost:5173",
                    "https://techmart.com",
                    "https://www.techmart.com"
                )
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        }
    );
});

// builder.Services.AddProblemDetails();

// Add health checks
// builder
//     .Services.AddHealthChecks()
//     .AddSqlServer(builder.Configuration.GetConnectionString("AuthDatabase")!)
//     .AddRedis(builder.Configuration.GetConnectionString("Redis")!);

// Add problem details
builder.Services.AddProblemDetails();

// Application layers
builder.Services.AddPresentation();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddEndpoints(Assembly.GetExecutingAssembly());

// Add API versioning
// builder.Services.AddApiVersioning(options =>
// {
//     options.ReportApiVersions = true;
//     options.AssumeDefaultVersionWhenUnspecified = true;
//     options.DefaultApiVersion = new Asp.Versioning.ApiVersion(1, 0);
// });

var app = builder.Build();

app.MapEndpoints();

app.UseSwaggerWithUi();

// Configure the HTTP request pipeline
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI(c =>
//     {
//         c.SwaggerEndpoint("/swagger/v1/swagger.json", "TechMart Auth API V1");
//         c.RoutePrefix = "swagger";
//     });

//     // Add Scalar UI
//     app.UseScalar(options =>
//     {
//         options.Title = "TechMart Auth API";
//         options.Theme = Scalar.AspNetCore.ScalarTheme.Purple;
//         options.ShowSidebar = true;
//         options.DefaultHttpClient = new Scalar.AspNetCore.ScalarTarget(
//             Scalar.AspNetCore.ScalarClient.Fetch
//         );
//         options.RoutePrefix = "scalar";
//     });
// }

// Security headers middleware
// app.UseSecurityHeaders();

// app.UseRequestContextLogging();

// app.UseSerilogRequestLogging();

// app.UseExceptionHandler();

// app.UseAuthentication();

// app.UseAuthorization();

app.UseHttpsRedirection();
app.UseCors("AllowedOrigins");

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// app.MapControllers();

// Request logging
// app.UseSerilogRequestLogging();

// Exception handling
app.UseExceptionHandler();

// Health checks
app.MapHealthChecks("/health");

// Map API endpoints
// app.MapApiEndpoints();

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
