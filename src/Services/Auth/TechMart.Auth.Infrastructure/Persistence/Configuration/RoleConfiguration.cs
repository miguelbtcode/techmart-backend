using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechMart.Auth.Domain.Roles.Constants;
using TechMart.Auth.Domain.Roles.Entities;
using TechMart.Auth.Domain.Roles.ValueObjects;

namespace TechMart.Auth.Infrastructure.Persistence.Configurations;

internal sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        ConfigureRolesTable(builder);
        ConfigureRelationships(builder);
        ConfigureIndexes(builder);
        SeedData(builder);
    }

    private static void ConfigureRolesTable(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles", "auth");

        builder.HasKey(r => r.Id);

        // Configure Id as strongly-typed value object
        builder
            .Property(r => r.Id)
            .HasConversion(id => id.Value, value => RoleId.From(value))
            .ValueGeneratedNever();

        builder.Property(r => r.Name).HasMaxLength(50).IsRequired();

        builder.Property(r => r.Description).HasMaxLength(500).IsRequired();

        builder.Property(r => r.HierarchyLevel).IsRequired();

        // Audit fields
        builder.Property(r => r.CreatedAt).IsRequired().HasDefaultValueSql("NOW()");

        builder.Property(r => r.UpdatedAt).IsRequired().HasDefaultValueSql("NOW()");
    }

    private static void ConfigureRelationships(EntityTypeBuilder<Role> builder)
    {
        // One-to-many relationship with UserRoles
        builder
            .HasMany(r => r.UserRoles)
            .WithOne(ur => ur.Role)
            .HasForeignKey(ur => ur.RoleId)
            .OnDelete(DeleteBehavior.Restrict); // Prevent deleting roles that are in use
    }

    private static void ConfigureIndexes(EntityTypeBuilder<Role> builder)
    {
        // Unique constraint on Name
        builder.HasIndex(r => r.Name).IsUnique().HasDatabaseName("IX_Roles_Name");

        // Index on HierarchyLevel for role hierarchy queries
        builder.HasIndex(r => r.HierarchyLevel).HasDatabaseName("IX_Roles_HierarchyLevel");
    }

    private static void SeedData(EntityTypeBuilder<Role> builder)
    {
        // Seed default system roles
        builder.HasData(
            new
            {
                Id = RoleConstants.AdministratorId,
                Name = RoleConstants.Administrator,
                Description = "Full system access with all permissions",
                HierarchyLevel = RoleConstants.AdministratorLevel,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            },
            new
            {
                Id = RoleConstants.CustomerId,
                Name = RoleConstants.Customer,
                Description = "Standard customer with shopping permissions",
                HierarchyLevel = RoleConstants.CustomerLevel,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            },
            new
            {
                Id = RoleConstants.ModeratorId,
                Name = RoleConstants.Moderator,
                Description = "Content moderation and user management permissions",
                HierarchyLevel = RoleConstants.ModeratorLevel,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            },
            new
            {
                Id = RoleConstants.SupportId,
                Name = RoleConstants.Support,
                Description = "Customer support and order management permissions",
                HierarchyLevel = RoleConstants.SupportLevel,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            }
        );
    }
}
