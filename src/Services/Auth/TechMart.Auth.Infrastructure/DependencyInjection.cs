using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using TechMart.Auth.Application.Abstractions.Authentication;
using TechMart.Auth.Application.Abstractions.Caching;
using TechMart.Auth.Application.Abstractions.Contracts;
using TechMart.Auth.Application.Abstractions.Events;
using TechMart.Auth.Application.Services;
using TechMart.Auth.Domain.Primitives;
using TechMart.Auth.Domain.Roles.Repository;
using TechMart.Auth.Domain.Users.Repository;
using TechMart.Auth.Infrastructure.Authentication;
using TechMart.Auth.Infrastructure.BackgroundServices;
using TechMart.Auth.Infrastructure.Caching;
using TechMart.Auth.Infrastructure.Events;
using TechMart.Auth.Infrastructure.Persistence;
using TechMart.Auth.Infrastructure.Persistence.Repositories;
using TechMart.Auth.Infrastructure.Services;
using TechMart.Auth.Infrastructure.Services.Settings;
using TechMart.Auth.Infrastructure.Settings;

namespace TechMart.Auth.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        // Configuration settings
        services.AddSettings(configuration);

        // Database
        services.AddDatabase(configuration);

        // Caching
        services.AddCaching(configuration);

        // Authentication & JWT
        services.AddAuthenticationServices();

        // Email services
        services.AddEmailServices();

        // Event handling (only if Kafka is configured)
        var kafkaAvailable = services.AddEventHandling(configuration);

        // Background services
        services.AddBackgroundServices();

        // Repositories
        services.AddRepositories();

        return services;
    }

    private static IServiceCollection AddSettings(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        // JWT Settings
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
        services.AddSingleton(provider =>
        {
            var jwtSettings =
                configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>()
                ?? throw new InvalidOperationException("JWT settings are not configured");
            jwtSettings.Validate();
            return jwtSettings;
        });

        // BCrypt Settings
        services.Configure<BCryptSettings>(configuration.GetSection(BCryptSettings.SectionName));
        services.AddSingleton(provider =>
        {
            var bcryptSettings =
                configuration.GetSection(BCryptSettings.SectionName).Get<BCryptSettings>()
                ?? new BCryptSettings();
            bcryptSettings.Validate();
            return bcryptSettings;
        });

        // Email Settings
        services.Configure<EmailSettings>(configuration.GetSection(EmailSettings.SectionName));
        services.AddSingleton(provider =>
        {
            var emailSettings =
                configuration.GetSection(EmailSettings.SectionName).Get<EmailSettings>()
                ?? throw new InvalidOperationException("Email settings are not configured");
            return emailSettings;
        });

        return services;
    }

    private static IServiceCollection AddDatabase(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var connectionString =
            configuration.GetConnectionString("AuthDatabase")
            ?? throw new InvalidOperationException(
                "Auth database connection string is not configured"
            );

        services.AddDbContext<AuthDbContext>(options =>
        {
            options.UseSqlServer(
                connectionString,
                sqlServerOptions =>
                {
                    sqlServerOptions.MigrationsAssembly(typeof(AuthDbContext).Assembly.FullName);
                    sqlServerOptions.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null
                    );
                    sqlServerOptions.CommandTimeout(30);
                }
            );

            // Enable detailed errors only in development
            options.EnableDetailedErrors();
            options.EnableSensitiveDataLogging(false);
        });

        // Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }

    private static bool AddCaching(this IServiceCollection services, IConfiguration configuration)
    {
        var redisConnectionString = configuration.GetConnectionString("Redis");

        if (!string.IsNullOrEmpty(redisConnectionString))
        {
            try
            {
                // Test connection first
                using var testConnection = ConnectionMultiplexer.Connect(redisConnectionString);
                testConnection.Dispose();

                // Redis connection
                services.AddSingleton<IConnectionMultiplexer>(provider =>
                {
                    var logger = provider.GetRequiredService<ILogger<ConnectionMultiplexer>>();
                    try
                    {
                        var connection = ConnectionMultiplexer.Connect(redisConnectionString);
                        connection.ConnectionFailed += (sender, args) =>
                            logger.LogError(
                                "Redis connection failed: {EndPoint} - {FailureType}",
                                args.EndPoint,
                                args.FailureType
                            );
                        connection.ConnectionRestored += (sender, args) =>
                            logger.LogInformation(
                                "Redis connection restored: {EndPoint}",
                                args.EndPoint
                            );
                        return connection;
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Failed to connect to Redis");
                        throw;
                    }
                });

                // Cache services
                services.AddSingleton<ICacheService, RedisCacheService>();
                services.AddSingleton<IRateLimitService, RedisRateLimitService>();

                Console.WriteLine("✅ Redis cache services registered successfully");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"⚠️ Warning: Redis not available, skipping cache services: {ex.Message}"
                );
                return false;
            }
        }
        else
        {
            Console.WriteLine(
                "⚠️ Warning: Redis connection string not configured, skipping cache services"
            );
            return false;
        }
    }

    private static IServiceCollection AddAuthenticationServices(this IServiceCollection services)
    {
        // Password hashing
        services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();

        // JWT provider
        services.AddSingleton<IJwtProvider, JwtProvider>();

        // Token services (combined implementation)
        services.AddScoped<IRefreshTokenService, RedisTokenService>();
        services.AddScoped<IPasswordResetService, RedisTokenService>();
        services.AddScoped<IEmailConfirmationService, RedisTokenService>();

        return services;
    }

    private static IServiceCollection AddEmailServices(this IServiceCollection services)
    {
        services.AddScoped<IEmailService, EmailService>();
        return services;
    }

    private static bool AddEventHandling(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        // Event serialization (always available)
        services.AddSingleton<IEventSerializer, JsonEventSerializer>();

        // Outbox pattern (always available)
        services.AddScoped<IOutboxService, OutboxService>();
        services.AddScoped<IOutboxRepository, OutboxRepository>();

        // Kafka producer (only if Kafka is configured)
        var kafkaConnectionString = configuration.GetConnectionString("Kafka");
        if (!string.IsNullOrEmpty(kafkaConnectionString))
        {
            try
            {
                services.AddSingleton<IKafkaProducer, KafkaProducer>();
                Console.WriteLine("✅ Kafka producer registered successfully");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"⚠️ Warning: Kafka not available, events will be stored in outbox only: {ex.Message}"
                );
                return false;
            }
        }
        else
        {
            Console.WriteLine(
                "⚠️ Warning: Kafka connection string not configured, events will be stored in outbox only"
            );
            return false;
        }
    }

    private static IServiceCollection AddBackgroundServices(this IServiceCollection services)
    {
        services.AddHostedService<OutboxPublisherService>();

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        // Register specific repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IUserRoleRepository, UserRoleRepository>();

        // Generic repository is handled by UnitOfWork

        return services;
    }

    /// <summary>
    /// Extension method to add database migration support
    /// </summary>
    public static async Task<IServiceProvider> MigrateDatabaseAsync(
        this IServiceProvider serviceProvider
    )
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<AuthDbContext>>();

        try
        {
            logger.LogInformation("Starting database migration...");
            await context.Database.MigrateAsync();
            logger.LogInformation("Database migration completed successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Database migration failed");
            throw;
        }

        return serviceProvider;
    }

    /// <summary>
    /// Extension method to seed initial data
    /// </summary>
    public static async Task<IServiceProvider> SeedDataAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<AuthDbContext>>();

        try
        {
            logger.LogInformation("Starting data seeding...");

            // Check if roles already exist
            var rolesExist = await context.Roles.AnyAsync();
            if (!rolesExist)
            {
                logger.LogInformation("Seeding initial roles...");
                // Roles are seeded through EF migrations/configuration
                await context.SaveChangesAsync();
            }

            logger.LogInformation("Data seeding completed successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Data seeding failed");
            throw;
        }

        return serviceProvider;
    }
}
