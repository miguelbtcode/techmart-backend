using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechMart.Product.Domain.Product.Entities;

namespace TechMart.Product.Infrastructure.Data.Configurations;

public class ProductVariantConfiguration : IEntityTypeConfiguration<ProductVariant>
{
    public void Configure(EntityTypeBuilder<ProductVariant> builder)
    {
        builder.ToTable("ProductVariants");
        builder.HasKey(pv => pv.Id);

        builder.OwnsOne(pv => pv.Sku, sku =>
        {
            sku.Property(s => s.Value)
                .HasColumnName("Sku")
                .HasMaxLength(20)
                .IsRequired();
        });

        builder.OwnsOne(pv => pv.Price, price =>
        {
            price.Property(p => p.Amount)
                .HasColumnName("Price")
                .HasColumnType("decimal(18,2)")
                .IsRequired();
            
            price.Property(p => p.Currency)
                .HasColumnName("Currency")
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.OwnsOne(pv => pv.CompareAtPrice, comparePrice =>
        {
            comparePrice.Property(p => p.Amount)
                .HasColumnName("CompareAtPrice")
                .HasColumnType("decimal(18,2)");
            
            comparePrice.Property(p => p.Currency)
                .HasColumnName("CompareAtCurrency")
                .HasMaxLength(3);
        });

        builder.Property(pv => pv.Name)
            .HasMaxLength(100)
            .IsRequired();

        // Store attributes as JSON
        builder.Property(pv => pv.Attributes)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null!),
                v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, (JsonSerializerOptions)null!) ?? new Dictionary<string, string>())
            .HasColumnType("jsonb");

        // Indexes
        builder.HasIndex(pv => pv.ProductId)
            .HasDatabaseName("IX_ProductVariants_ProductId");

        builder.HasIndex(pv => pv.IsActive)
            .HasDatabaseName("IX_ProductVariants_IsActive");
    }
}