using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechMart.Product.Domain.Product.Entities;

namespace TechMart.Product.Infrastructure.Data.EntityFramework.Configurations;

public class ProductAttributeConfiguration : IEntityTypeConfiguration<ProductAttribute>
{
    public void Configure(EntityTypeBuilder<ProductAttribute> builder)
    {
        builder.ToTable("ProductAttributes");
        builder.HasKey(pa => pa.Id);

        builder.Property(pa => pa.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(pa => pa.Value)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(pa => pa.DisplayName)
            .HasMaxLength(150);

        builder.Property(pa => pa.Unit)
            .HasMaxLength(20);

        // Indexes
        builder.HasIndex(pa => pa.ProductId)
            .HasDatabaseName("IX_ProductAttributes_ProductId");

        builder.HasIndex(pa => new { pa.ProductId, pa.Name })
            .IsUnique()
            .HasDatabaseName("IX_ProductAttributes_ProductId_Name");

        builder.HasIndex(pa => pa.IsVisible)
            .HasDatabaseName("IX_ProductAttributes_IsVisible");
    }
}