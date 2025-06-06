using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechMart.Product.Domain.Inventory;

namespace TechMart.Product.Infrastructure.Data.Configurations;

public class InventoryConfiguration : IEntityTypeConfiguration<Inventory>
{
    public void Configure(EntityTypeBuilder<Inventory> builder)
    {
        builder.ToTable("Inventories");
        builder.HasKey(i => i.Id);

        builder.Property(i => i.ProductSku)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(i => i.Status)
            .HasConversion<int>()
            .IsRequired();

        // Relationship with Product
        builder.HasIndex(i => i.ProductId)
            .IsUnique()
            .HasDatabaseName("IX_Inventories_ProductId");

        builder.HasIndex(i => i.ProductSku)
            .IsUnique()
            .HasDatabaseName("IX_Inventories_ProductSku");

        // Child entities
        builder.HasMany(i => i.Transactions)
            .WithOne()
            .HasForeignKey("InventoryId")
            .OnDelete(DeleteBehavior.Cascade);

        // Ignore domain events
        builder.Ignore(i => i.DomainEvents);
    }
}