using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechMart.Auth.Domain.Primitives;

namespace TechMart.Auth.Infrastructure.Persistence.Configuration;

internal sealed class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("OutboxMessages", "auth");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.EventId).IsRequired();

        builder.Property(x => x.EventType).HasMaxLength(255).IsRequired();

        builder.Property(x => x.EventData).IsRequired();

        builder.Property(x => x.OccurredAt).IsRequired();

        builder.Property(x => x.ProcessedAt).IsRequired(false);

        builder.Property(x => x.Error).HasMaxLength(1000).IsRequired(false);

        builder.Property(x => x.RetryCount).HasDefaultValue(0).IsRequired();

        // Audit fields
        builder.Property(x => x.CreatedAt).IsRequired().HasDefaultValueSql("GETUTCDATE()");

        builder.Property(x => x.UpdatedAt).IsRequired().HasDefaultValueSql("GETUTCDATE()");

        // Indexes
        builder.HasIndex(x => x.ProcessedAt).HasDatabaseName("IX_OutboxMessages_ProcessedAt");

        builder.HasIndex(x => x.OccurredAt).HasDatabaseName("IX_OutboxMessages_OccurredAt");

        builder.HasIndex(x => x.EventType).HasDatabaseName("IX_OutboxMessages_EventType");

        builder
            .HasIndex(x => new { x.ProcessedAt, x.RetryCount })
            .HasDatabaseName("IX_OutboxMessages_ProcessedAt_RetryCount");
    }
}
