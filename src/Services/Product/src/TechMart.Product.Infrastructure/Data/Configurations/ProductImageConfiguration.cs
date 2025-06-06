using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechMart.Product.Domain.Product.Entities;

namespace TechMart.Product.Infrastructure.Data.Configurations;

public class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
{
    public void Configure(EntityTypeBuilder<ProductImage> builder)
    {
        builder.ToTable("ProductImages");
        builder.HasKey(pi => pi.Id);

        builder.OwnsOne(pi => pi.ImageUrl, imageUrl =>
        {
            imageUrl.Property(iu => iu.Value)
                .HasColumnName("ImageUrl")
                .HasMaxLength(500)
                .IsRequired();
        });

        builder.Property(pi => pi.AltText)
            .HasMaxLength(200);

        // Indexes
        builder.HasIndex(pi => pi.ProductId)
            .HasDatabaseName("IX_ProductImages_ProductId");

        builder.HasIndex(pi => pi.IsPrimary)
            .HasDatabaseName("IX_ProductImages_IsPrimary");

        builder.HasIndex(pi => pi.SortOrder)
            .HasDatabaseName("IX_ProductImages_SortOrder");
    }
}