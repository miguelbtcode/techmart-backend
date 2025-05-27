using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechMart.Auth.Domain.Roles.Entities;
using TechMart.Auth.Domain.Roles.ValueObjects;
using TechMart.Auth.Domain.Users.ValueObjects;

namespace TechMart.Auth.Infrastructure.Persistence.Configurations;

internal sealed class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        ConfigureUserRolesTable(builder);
        ConfigureRelationships(builder);
        ConfigureIndexes(builder);
    }

    private static void ConfigureUserRolesTable(EntityTypeBuilder<UserRole> builder)
    {
        builder.ToTable("UserRoles", "auth");

        builder.HasKey(ur => ur.Id);

        // Configure Id as strongly-typed value object
        builder
            .Property(ur => ur.Id)
            .HasConversion(id => id.Value, value => UserRoleId.From(value))
            .ValueGeneratedNever();

        // Configure UserId as strongly-typed value object
        builder
            .Property(ur => ur.UserId)
            .HasConversion(id => id.Value, value => UserId.From(value))
            .IsRequired();

        // Configure RoleId as strongly-typed value object
        builder
            .Property(ur => ur.RoleId)
            .HasConversion(id => id.Value, value => RoleId.From(value))
            .IsRequired();

        builder.Property(ur => ur.AssignedAt).IsRequired();

        // Audit fields - SQL Server syntax
        builder.Property(ur => ur.CreatedAt).IsRequired().HasDefaultValueSql("GETUTCDATE()");

        builder.Property(ur => ur.UpdatedAt).IsRequired().HasDefaultValueSql("GETUTCDATE()");

        builder.Property(ur => ur.CreatedBy).IsRequired(false);

        builder.Property(ur => ur.UpdatedBy).IsRequired(false);
    }

    private static void ConfigureRelationships(EntityTypeBuilder<UserRole> builder)
    {
        // Many-to-one relationship with User
        builder
            .HasOne(ur => ur.User)
            .WithMany(u => u.UserRoles)
            .HasForeignKey(ur => ur.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Many-to-one relationship with Role
        builder
            .HasOne(ur => ur.Role)
            .WithMany(r => r.UserRoles)
            .HasForeignKey(ur => ur.RoleId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    private static void ConfigureIndexes(EntityTypeBuilder<UserRole> builder)
    {
        // Unique constraint on UserId + RoleId combination
        builder
            .HasIndex(ur => new { ur.UserId, ur.RoleId })
            .IsUnique()
            .HasDatabaseName("IX_UserRoles_UserId_RoleId");

        // Index for querying user roles
        builder.HasIndex(ur => ur.UserId).HasDatabaseName("IX_UserRoles_UserId");

        // Index for querying role assignments
        builder.HasIndex(ur => ur.RoleId).HasDatabaseName("IX_UserRoles_RoleId");

        // Index for querying by assignment date
        builder.HasIndex(ur => ur.AssignedAt).HasDatabaseName("IX_UserRoles_AssignedAt");
    }
}
