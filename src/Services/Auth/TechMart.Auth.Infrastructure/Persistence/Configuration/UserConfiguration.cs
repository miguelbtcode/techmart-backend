using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechMart.Auth.Domain.Users.Entities;
using TechMart.Auth.Domain.Users.ValueObjects;

namespace TechMart.Auth.Infrastructure.Persistence.Configurations;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        ConfigureUsersTable(builder);
        ConfigureValueObjects(builder);
        ConfigureRelationships(builder);
        ConfigureIndexes(builder);
    }

    private static void ConfigureUsersTable(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users", "auth");

        builder.HasKey(u => u.Id);

        // Configure Id as strongly-typed value object
        builder
            .Property(u => u.Id)
            .HasConversion(id => id.Value, value => UserId.From(value))
            .ValueGeneratedNever();

        builder.Property(u => u.FirstName).HasMaxLength(100).IsRequired();

        builder.Property(u => u.LastName).HasMaxLength(100).IsRequired();

        builder.Property(u => u.PasswordHash).HasMaxLength(500).IsRequired();

        builder.Property(u => u.Status).HasConversion<string>().HasMaxLength(50).IsRequired();

        builder.Property(u => u.IsEmailConfirmed).IsRequired().HasDefaultValue(false);

        builder.Property(u => u.LastLoginAt).IsRequired(false);

        // Audit fields
        builder.Property(u => u.CreatedAt).IsRequired().HasDefaultValueSql("NOW()");

        builder.Property(u => u.UpdatedAt).IsRequired().HasDefaultValueSql("NOW()");

        builder.Property(u => u.CreatedBy).IsRequired(false);

        builder.Property(u => u.UpdatedBy).IsRequired(false);
    }

    private static void ConfigureValueObjects(EntityTypeBuilder<User> builder)
    {
        // Configure Email value object
        builder
            .Property(u => u.Email)
            .HasConversion(email => email.Value, value => Email.Create(value).Value) // This should be safe since we're loading from DB
            .HasMaxLength(255)
            .IsRequired();
    }

    private static void ConfigureRelationships(EntityTypeBuilder<User> builder)
    {
        // One-to-many relationship with UserRoles
        builder
            .HasMany(u => u.UserRoles)
            .WithOne(ur => ur.User)
            .HasForeignKey(ur => ur.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    private static void ConfigureIndexes(EntityTypeBuilder<User> builder)
    {
        // Unique constraint on Email
        builder.HasIndex(u => u.Email).IsUnique().HasDatabaseName("IX_Users_Email");

        // Index for frequently queried fields
        builder.HasIndex(u => u.Status).HasDatabaseName("IX_Users_Status");

        builder.HasIndex(u => u.CreatedAt).HasDatabaseName("IX_Users_CreatedAt");

        builder.HasIndex(u => u.LastLoginAt).HasDatabaseName("IX_Users_LastLoginAt");
    }
}
