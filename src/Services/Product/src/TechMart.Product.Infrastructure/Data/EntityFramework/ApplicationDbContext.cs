using System.Text;
using Microsoft.EntityFrameworkCore;
using TechMart.Product.Domain.Brand;
using TechMart.Product.Domain.Category;
using TechMart.Product.Domain.Inventory;
using TechMart.Product.Domain.Inventory.Entities;
using TechMart.Product.Domain.Product.Entities;
using TechMart.Product.Infrastructure.Data.EntityFramework.Interceptors;
using ProductEntity = TechMart.Product.Domain.Product.Product;

namespace TechMart.Product.Infrastructure.Data.EntityFramework;

public class ApplicationDbContext : DbContext
{
    private readonly AuditableEntityInterceptor _auditableEntityInterceptor;
    private readonly DomainEventInterceptor _domainEventInterceptor;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        AuditableEntityInterceptor auditableEntityInterceptor,
        DomainEventInterceptor domainEventInterceptor) : base(options)
    {
        _auditableEntityInterceptor = auditableEntityInterceptor;
        _domainEventInterceptor = domainEventInterceptor;
    }

    // Main aggregate roots
    public DbSet<ProductEntity> Products => Set<ProductEntity>();
    public DbSet<Brand> Brands => Set<Brand>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Inventory> Inventories => Set<Inventory>();

    // Child entities
    public DbSet<ProductImage> ProductImages => Set<ProductImage>();
    public DbSet<ProductVariant> ProductVariants => Set<ProductVariant>();
    public DbSet<ProductAttribute> ProductAttributes => Set<ProductAttribute>();
    public DbSet<ProductReview> ProductReviews => Set<ProductReview>();
    public DbSet<InventoryTransaction> InventoryTransactions => Set<InventoryTransaction>();


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(_auditableEntityInterceptor, _domainEventInterceptor);
        
        // PostgreSQL specific configurations
        optionsBuilder.UseNpgsql(options =>
        {
            options.EnableRetryOnFailure(
                maxRetryCount: 3,
                maxRetryDelay: TimeSpan.FromSeconds(5),
                errorCodesToAdd: null);
        });

        // Development configurations
        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
        {
            optionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder.EnableDetailedErrors();
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ApplyConfigurations(modelBuilder);
        // ApplyGlobalQueryFilters(modelBuilder);
        ConfigureDatabaseSpecificSettings(modelBuilder);
    }
    
    private static void ApplyConfigurations(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    private static void ConfigureDatabaseSpecificSettings(ModelBuilder modelBuilder)
    {
        // PostgreSQL specific configurations
        ConfigurePostgreSQLSettings(modelBuilder);

        // Configure decimal precision globally
        ConfigureDecimalPrecision(modelBuilder);

        // Configure string lengths globally
        ConfigureStringLengths(modelBuilder);
    }
    
    private static void ConfigurePostgreSQLSettings(ModelBuilder modelBuilder)
    {
        // Use snake_case naming convention for PostgreSQL
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            // Convert table names to snake_case
            entity.SetTableName(entity.GetTableName()?.ToSnakeCase());

            // Convert column names to snake_case
            foreach (var property in entity.GetProperties())
            {
                property.SetColumnName(property.GetColumnName().ToSnakeCase());
            }

            // Convert index names to snake_case
            foreach (var index in entity.GetIndexes())
            {
                index.SetDatabaseName(index.GetDatabaseName()?.ToSnakeCase());
            }

            // Convert foreign key names to snake_case
            foreach (var foreignKey in entity.GetForeignKeys())
            {
                foreignKey.SetConstraintName(foreignKey.GetConstraintName()?.ToSnakeCase());
            }
        }
    }
    
    private static void ConfigureDecimalPrecision(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(decimal) || property.ClrType == typeof(decimal?))
                {
                    // Set default precision for decimal properties
                    if (!property.GetColumnType().Contains("decimal"))
                    {
                        property.SetColumnType("decimal(18,2)");
                    }
                }
            }
        }
    }
    
    private static void ConfigureStringLengths(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(string))
                {
                    var maxLength = property.GetMaxLength();
                    if (!maxLength.HasValue)
                    {
                        // Set default max length for string properties without explicit length
                        property.SetMaxLength(500);
                    }
                }
            }
        }
    }
}

// Extension method for snake_case conversion
public static class StringExtensions
{
    public static string ToSnakeCase(this string text)
    {
        if (string.IsNullOrEmpty(text))
            return text;

        var result = new StringBuilder();
        result.Append(char.ToLowerInvariant(text[0]));

        for (int i = 1; i < text.Length; i++)
        {
            char c = text[i];
            if (char.IsUpper(c))
            {
                result.Append('_');
                result.Append(char.ToLowerInvariant(c));
            }
            else
            {
                result.Append(c);
            }
        }

        return result.ToString();
    }
}