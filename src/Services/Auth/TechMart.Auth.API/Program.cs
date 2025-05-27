using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using TechMart.Auth.API.Extensions;
using TechMart.Auth.Application;
using TechMart.Auth.Infrastructure;
using TechMart.Auth.Infrastructure.Settings;

var builder = WebApplication.CreateBuilder(args);

// Logging configuration
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

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

// Add OpenAPI/Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc(
        "v1",
        new OpenApiInfo
        {
            Title = "TechMart Auth API",
            Version = "v1",
            Description =
                "Authentication and authorization service for TechMart e-commerce platform",
            Contact = new OpenApiContact { Name = "TechMart Team", Email = "support@techmart.com" },
        }
    );

    // Add JWT authentication to Swagger
    c.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            Description =
                "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
        }
    );

    c.AddSecurityRequirement(
        new OpenApiSecurityRequirement
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
        }
    );

    // Add XML comments if available
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
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
                    "http://localhost:5173", // Vite dev server
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

// Add problem details
builder.Services.AddProblemDetails();

// Application layers
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Add API versioning
builder.Services.AddApiVersioning(options =>
{
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new Asp.Versioning.ApiVersion(1, 0);
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "TechMart Auth API V1");
        c.RoutePrefix = "swagger";
    });

    // Add Scalar UI
    app.UseScalar(options =>
    {
        options.Title = "TechMart Auth API";
        options.Theme = Scalar.AspNetCore.ScalarTheme.Purple;
        options.ShowSidebar = true;
        options.DefaultHttpClient = new Scalar.AspNetCore.ScalarTarget(
            Scalar.AspNetCore.ScalarClient.Fetch
        );
        options.RoutePrefix = "scalar";
    });
}

// Security headers middleware
app.UseSecurityHeaders();

app.UseHttpsRedirection();
app.UseCors("AllowedOrigins");

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// Request logging
app.UseSerilogRequestLogging();

// Exception handling
app.UseExceptionHandler();

// Health checks
app.MapHealthChecks("/health");

// Map API endpoints
app.MapApiEndpoints();

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
