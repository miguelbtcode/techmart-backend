using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechMart.Product.Domain.Product.Entities;

namespace TechMart.Product.Infrastructure.Data.Configurations;

public class ProductReviewConfiguration : IEntityTypeConfiguration<ProductReview>
{
    public void Configure(EntityTypeBuilder<ProductReview> builder)
    {
        builder.ToTable("ProductReviews");
        builder.HasKey(pr => pr.Id);

        builder.Property(pr => pr.CustomerName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(pr => pr.CustomerEmail)
            .HasMaxLength(200);

        builder.Property(pr => pr.Title)
            .HasMaxLength(200);

        builder.Property(pr => pr.Comment)
            .HasMaxLength(2000);

        builder.Property(pr => pr.ModeratorNotes)
            .HasMaxLength(1000);

        builder.Property(pr => pr.Status)
            .HasConversion<int>()
            .IsRequired();

        // Indexes
        builder.HasIndex(pr => pr.ProductId)
            .HasDatabaseName("IX_ProductReviews_ProductId");

        builder.HasIndex(pr => pr.Status)
            .HasDatabaseName("IX_ProductReviews_Status");

        builder.HasIndex(pr => pr.Rating)
            .HasDatabaseName("IX_ProductReviews_Rating");

        builder.HasIndex(pr => pr.IsVerifiedPurchase)
            .HasDatabaseName("IX_ProductReviews_IsVerifiedPurchase");

        builder.HasIndex(pr => pr.CreatedAt)
            .HasDatabaseName("IX_ProductReviews_CreatedAt");
    }
}