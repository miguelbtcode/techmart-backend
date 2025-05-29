using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace AuthMicroservice.Infrastructure.Data;

/// <summary>
/// Factory para crear DbContext en tiempo de diseño (migraciones)
/// Evita construir toda la aplicación solo para generar migraciones
/// </summary>
public class AuthDbContextFactory : IDesignTimeDbContextFactory<AuthDbContext>
{
    public AuthDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AuthDbContext>();

        // Intentar cargar configuración desde appsettings.json
        var connectionString = GetConnectionString();

        optionsBuilder.UseSqlServer(
            connectionString,
            sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null
                );
                sqlOptions.CommandTimeout(30);
            }
        );

        return new AuthDbContext(optionsBuilder.Options);
    }

    private static string GetConnectionString()
    {
        // Opción 1: Intentar leer desde appsettings.json del proyecto API
        try
        {
            var basePath = GetApiProjectPath();
            if (Directory.Exists(basePath))
            {
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(basePath)
                    .AddJsonFile("appsettings.json", optional: false)
                    .AddJsonFile("appsettings.Development.json", optional: true)
                    .Build();

                var connectionString = configuration.GetConnectionString("DefaultConnection");
                if (!string.IsNullOrEmpty(connectionString))
                {
                    Console.WriteLine($"✅ Using connection string from appsettings.json");
                    return connectionString;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️ Could not load appsettings.json: {ex.Message}");
        }

        // Opción 2: Usar variable de entorno
        var envConnectionString = Environment.GetEnvironmentVariable(
            "ConnectionStrings__DefaultConnection"
        );
        if (!string.IsNullOrEmpty(envConnectionString))
        {
            Console.WriteLine("✅ Using connection string from environment variable");
            return envConnectionString;
        }

        // Opción 3: Connection string por defecto para desarrollo
        var defaultConnectionString = GetDefaultConnectionString();
        Console.WriteLine("⚠️ Using default connection string for development");
        return defaultConnectionString;
    }

    private static string GetApiProjectPath()
    {
        // Buscar el proyecto API desde diferentes ubicaciones posibles
        var currentDirectory = Directory.GetCurrentDirectory();

        // Caso 1: Ejecutando desde Infrastructure
        var apiPath1 = Path.GetFullPath(Path.Combine(currentDirectory, "../AuthMicroservice.API"));
        if (Directory.Exists(apiPath1))
            return apiPath1;

        // Caso 2: Ejecutando desde raíz
        var apiPath2 = Path.GetFullPath(Path.Combine(currentDirectory, "AuthMicroservice.API"));
        if (Directory.Exists(apiPath2))
            return apiPath2;

        // Caso 3: Buscar recursivamente hacia arriba
        var directory = new DirectoryInfo(currentDirectory);
        while (directory != null)
        {
            var apiPath3 = Path.Combine(directory.FullName, "AuthMicroservice.API");
            if (Directory.Exists(apiPath3))
                return apiPath3;

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException(
            "Could not find AuthMicroservice.API project directory"
        );
    }

    private static string GetDefaultConnectionString()
    {
        // Para macOS/Linux con Docker SQL Server
        if (Environment.OSVersion.Platform == PlatformID.Unix)
        {
            return "Server=localhost,1433;Database=AuthMicroserviceDb;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=true;";
        }

        // Para Windows con LocalDB
        return "Server=(localdb)\\mssqllocaldb;Database=AuthMicroserviceDb;Trusted_Connection=true;MultipleActiveResultSets=true";
    }
}
