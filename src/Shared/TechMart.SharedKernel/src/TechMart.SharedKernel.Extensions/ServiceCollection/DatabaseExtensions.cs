using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TechMart.SharedKernel.Extensions.ServiceCollection;

/// <summary>
/// Extension methods for configuring database connections.
/// </summary>
public static class DatabaseExtensions
{
    /// <summary>
    /// Adds SQL Server with TechMart configuration.
    /// </summary>
    /// <typeparam name="TContext">The DbContext type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <param name="connectionStringName">The connection string name.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddTechMartSqlServer<TContext>(
        this IServiceCollection services,
        IConfiguration configuration,
        string connectionStringName = "DefaultConnection")
        where TContext : DbContext
    {
        var connectionString = configuration.GetConnectionString(connectionStringName)
            ?? throw new InvalidOperationException($"Connection string '{connectionStringName}' not found.");

        services.AddDbContext<TContext>(options =>
        {
            options.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorNumbersToAdd: null);
                
                sqlOptions.CommandTimeout(30);
            });

            options.EnableServiceProviderCaching();
            options.EnableSensitiveDataLogging(false);
        });

        return services;
    }

    /// <summary>
    /// Adds PostgreSQL with TechMart configuration.
    /// </summary>
    /// <typeparam name="TContext">The DbContext type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <param name="connectionStringName">The connection string name.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddTechMartPostgreSQL<TContext>(
        this IServiceCollection services,
        IConfiguration configuration,
        string connectionStringName = "DefaultConnection")
        where TContext : DbContext
    {
        var connectionString = configuration.GetConnectionString(connectionStringName)
            ?? throw new InvalidOperationException($"Connection string '{connectionStringName}' not found.");

        services.AddDbContext<TContext>(options =>
        {
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorCodesToAdd: null);
                
                npgsqlOptions.CommandTimeout(30);
            });

            options.EnableServiceProviderCaching();
            options.EnableSensitiveDataLogging(false);
        });

        return services;
    }

    /// <summary>
    /// Adds MySQL with TechMart configuration.
    /// </summary>
    /// <typeparam name="TContext">The DbContext type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <param name="connectionStringName">The connection string name.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddTechMartMySQL<TContext>(
        this IServiceCollection services,
        IConfiguration configuration,
        string connectionStringName = "DefaultConnection")
        where TContext : DbContext
    {
        var connectionString = configuration.GetConnectionString(connectionStringName)
            ?? throw new InvalidOperationException($"Connection string '{connectionStringName}' not found.");

        services.AddDbContext<TContext>(options =>
        {
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), mySqlOptions =>
            {
                mySqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorNumbersToAdd: null);
                
                mySqlOptions.CommandTimeout(30);
            });

            options.EnableServiceProviderCaching();
            options.EnableSensitiveDataLogging(false);
        });

        return services;
    }

    /// <summary>
    /// Adds MongoDB connection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <param name="connectionStringName">The connection string name.</param>
    /// <param name="databaseName">The database name.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddTechMartMongoDB(
        this IServiceCollection services,
        IConfiguration configuration,
        string connectionStringName = "MongoConnection",
        string databaseName = "TechMartDb")
    {
        var connectionString = configuration.GetConnectionString(connectionStringName)
            ?? throw new InvalidOperationException($"Connection string '{connectionStringName}' not found.");

        services.AddSingleton<MongoDB.Driver.IMongoClient>(sp =>
            new MongoDB.Driver.MongoClient(connectionString));

        services.AddScoped(sp =>
            sp.GetRequiredService<MongoDB.Driver.IMongoClient>().GetDatabase(databaseName));

        return services;
    }
}