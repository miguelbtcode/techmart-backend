using Microsoft.EntityFrameworkCore;
using TechMart.Auth.Domain.Primitives;
using TechMart.Auth.Domain.Roles.Entities;
using TechMart.Auth.Domain.Users.Entities;

namespace TechMart.Auth.Infrastructure.Persistence;

public sealed class AuthDbContext : DbContext
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options)
        : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Apply all configurations from the assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AuthDbContext).Assembly);

        // Apply global query filters for soft delete
        ApplyGlobalQueryFilters(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }

    private static void ApplyGlobalQueryFilters(ModelBuilder modelBuilder)
    {
        // Global query filters can be added here if needed
        // For example, for soft delete functionality
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Update audit fields before saving
        UpdateAuditFields();

        var result = await base.SaveChangesAsync(cancellationToken);

        return result;
    }

    private void UpdateAuditFields()
    {
        var entries = ChangeTracker
            .Entries()
            .Where(e =>
                e.Entity is Entity<object>
                && (e.State == EntityState.Added || e.State == EntityState.Modified)
            );

        foreach (var entityEntry in entries)
        {
            var entity = (Entity<object>)entityEntry.Entity;

            if (entityEntry.State == EntityState.Added)
            {
                // CreatedAt and UpdatedAt are set in the Entity base class constructor
                // CreatedBy should be set by the application layer
            }
            else if (entityEntry.State == EntityState.Modified)
            {
                // Use reflection to update UpdatedAt
                var updateProperty = entityEntry.Property(nameof(Entity<object>.UpdatedAt));
                if (updateProperty != null)
                {
                    updateProperty.CurrentValue = DateTime.UtcNow;
                }
            }
        }
    }
}
