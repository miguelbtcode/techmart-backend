using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechMart.Product.Domain.Brand;

namespace TechMart.Product.Infrastructure.Data.EntityFramework.Configurations;

public class BrandConfiguration : IEntityTypeConfiguration<Brand>
{
    public void Configure(EntityTypeBuilder<Brand> builder)
    {
        builder.ToTable("Brands");
        builder.HasKey(b => b.Id);

        builder.Property(b => b.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(b => b.Description)
            .HasMaxLength(500);

        builder.Property(b => b.Slug)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(b => b.LogoUrl)
            .HasMaxLength(500);

        builder.Property(b => b.WebsiteUrl)
            .HasMaxLength(500);

        // Indexes
        builder.HasIndex(b => b.Name)
            .IsUnique()
            .HasDatabaseName("IX_Brands_Name");

        builder.HasIndex(b => b.Slug)
            .IsUnique()
            .HasDatabaseName("IX_Brands_Slug");

        builder.HasIndex(b => b.IsActive)
            .HasDatabaseName("IX_Brands_IsActive");

        // Ignore domain events
        builder.Ignore(b => b.DomainEvents);
    }
}