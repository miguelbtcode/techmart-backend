using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechMart.Product.Domain.Brand;
using TechMart.Product.Domain.Category;
using ProductEntity = TechMart.Product.Domain.Product.Product;

namespace TechMart.Product.Infrastructure.Data.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<ProductEntity>
{
    public void Configure(EntityTypeBuilder<ProductEntity> builder)
    {
        // Table configuration
        builder.ToTable("Products");
        builder.HasKey(p => p.Id);

        // Value object configurations
        builder.OwnsOne(p => p.Sku, sku =>
        {
            sku.Property(s => s.Value)
                .HasColumnName("Sku")
                .HasMaxLength(20)
                .IsRequired();
            
            sku.HasIndex(s => s.Value)
                .IsUnique()
                .HasDatabaseName("IX_Products_Sku");
        });

        builder.OwnsOne(p => p.Price, price =>
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

        builder.OwnsOne(p => p.CompareAtPrice, comparePrice =>
        {
            comparePrice.Property(p => p.Amount)
                .HasColumnName("CompareAtPrice")
                .HasColumnType("decimal(18,2)");
            
            comparePrice.Property(p => p.Currency)
                .HasColumnName("CompareAtCurrency")
                .HasMaxLength(3);
        });

        builder.OwnsOne(p => p.Weight, weight =>
        {
            weight.Property(w => w.Value)
                .HasColumnName("Weight")
                .HasColumnType("decimal(10,3)");
            
            weight.Property(w => w.Unit)
                .HasColumnName("WeightUnit")
                .HasMaxLength(10);
        });

        builder.OwnsOne(p => p.Dimensions, dimensions =>
        {
            dimensions.Property(d => d.Length)
                .HasColumnName("Length")
                .HasColumnType("decimal(10,2)");
            
            dimensions.Property(d => d.Width)
                .HasColumnName("Width")
                .HasColumnType("decimal(10,2)");
            
            dimensions.Property(d => d.Height)
                .HasColumnName("Height")
                .HasColumnType("decimal(10,2)");
            
            dimensions.Property(d => d.Unit)
                .HasColumnName("DimensionsUnit")
                .HasMaxLength(10);
        });

        // Basic properties
        builder.Property(p => p.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(p => p.Description)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(p => p.ShortDescription)
            .HasMaxLength(500);

        builder.Property(p => p.Tags)
            .HasMaxLength(1000);

        builder.Property(p => p.SeoTitle)
            .HasMaxLength(100);

        builder.Property(p => p.SeoDescription)
            .HasMaxLength(300);

        // Enum conversions
        builder.Property(p => p.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(p => p.Type)
            .HasConversion<int>()
            .IsRequired();

        // Relationships
        builder.HasOne<Brand>()
            .WithMany()
            .HasForeignKey(p => p.BrandId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Category>()
            .WithMany()
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // Child entities
        builder.HasMany(p => p.Images)
            .WithOne()
            .HasForeignKey(pi => pi.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.Variants)
            .WithOne()
            .HasForeignKey(pv => pv.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.Attributes)
            .WithOne()
            .HasForeignKey(pa => pa.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.Reviews)
            .WithOne()
            .HasForeignKey(pr => pr.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(p => p.Name)
            .HasDatabaseName("IX_Products_Name");

        builder.HasIndex(p => p.Status)
            .HasDatabaseName("IX_Products_Status");

        builder.HasIndex(p => p.IsActive)
            .HasDatabaseName("IX_Products_IsActive");

        builder.HasIndex(p => p.IsFeatured)
            .HasDatabaseName("IX_Products_IsFeatured");

        builder.HasIndex(p => p.BrandId)
            .HasDatabaseName("IX_Products_BrandId");

        builder.HasIndex(p => p.CategoryId)
            .HasDatabaseName("IX_Products_CategoryId");

        builder.HasIndex(p => p.CreatedAt)
            .HasDatabaseName("IX_Products_CreatedAt");

        // Ignore domain events
        builder.Ignore(p => p.DomainEvents);
    }
}