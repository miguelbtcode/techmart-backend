using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Asp.Versioning;
using AuthMicroservice.Application;
using AuthMicroservice.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

namespace AuthMicroservice.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        // Infrastructure Services (Database, JWT, Email, Social Auth, Repositories, etc.)
        services.AddInfrastructure(configuration);

        // Application Services (MediatR, Behaviors, FluentValidation)
        services.AddApplication();

        // API-specific Configuration
        services.AddApiConfiguration();

        return services;
    }

    private static IServiceCollection AddApiConfiguration(this IServiceCollection services)
    {
        // Controllers with JSON configuration
        services
            .AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.DefaultIgnoreCondition =
                    JsonIgnoreCondition.WhenWritingNull;
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

        services.AddEndpointsApiExplorer();

        // Swagger Configuration
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc(
                "v1",
                new OpenApiInfo
                {
                    Title = "Auth Microservice API",
                    Version = "v1",
                    Description =
                        "Authentication and User Management API with JWT, Social Auth, and Email Verification",
                    Contact = new OpenApiContact
                    {
                        Name = "Auth API Support",
                        Email = "support@authapi.com",
                    },
                    License = new OpenApiLicense
                    {
                        Name = "MIT License",
                        Url = new Uri("https://opensource.org/licenses/MIT"),
                    },
                }
            );

            // JWT Authentication
            c.AddSecurityDefinition(
                "Bearer",
                new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description =
                        "Enter 'Bearer' [space] and then your valid token in the text input below.\r\n\r\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...\"",
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

            // Include XML comments if available
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                c.IncludeXmlComments(xmlPath);
            }

            // Enable annotations
            c.EnableAnnotations();
        });

        // CORS Configuration
        services.AddCors(options =>
        {
            options.AddPolicy(
                "Development",
                policy =>
                {
                    policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                }
            );

            options.AddPolicy(
                "Production",
                policy =>
                {
                    policy
                        .WithOrigins("https://yourdomain.com", "https://www.yourdomain.com")
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                }
            );
        });

        // API Versioning (optional)
        services
            .AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ApiVersionReader = ApiVersionReader.Combine(
                    new UrlSegmentApiVersionReader(),
                    new HeaderApiVersionReader("X-Version"),
                    new QueryStringApiVersionReader("version")
                );
            })
            .AddApiExplorer(setup =>
            {
                setup.GroupNameFormat = "'v'VVV";
                setup.SubstituteApiVersionInUrl = true;
            });

        return services;
    }
}
