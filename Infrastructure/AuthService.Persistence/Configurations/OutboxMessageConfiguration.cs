/**
 * OutboxMessageConfiguration defines EF Core mapping for OutboxMessage entity.
 *
 * <p>Configures database schema for storing domain events before publishing.</p>
 */

namespace AuthService.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using AuthService.Persistence.Entities;


/// <summary>
/// EF Core configuration for <see cref="OutboxMessage"/> entity.
/// </summary>
public class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("OutboxMessages");

        builder.HasKey(x => x.Id);

        /// <summary>
        /// Timestamp when the domain event occurred.
        /// </summary>
        builder.Property(x => x.OccurredOnUtc)
            .IsRequired();

        /// <summary>
        /// Full type name of the domain event (for deserialization).
        /// </summary>
        builder.Property(x => x.Type)
            .IsRequired()
            .HasMaxLength(500);

        /// <summary>
        /// JSON-serialized domain event payload.
        /// </summary>
        builder.Property(x => x.Content)
            .IsRequired();

        /// <summary>
        /// Timestamp when the message was processed (null if pending).
        /// </summary>
        builder.Property(x => x.ProcessedOnUtc)
            .IsRequired(false);

        /// <summary>
        /// Error message if processing failed (null if successful).
        /// </summary>
        builder.Property(x => x.Error)
            .IsRequired(false)
            .HasMaxLength(2000);

        // Index for querying unprocessed messages efficiently
        builder.HasIndex(x => x.ProcessedOnUtc)
            .HasDatabaseName("IX_OutboxMessages_ProcessedOnUtc")
            .HasFilter("[ProcessedOnUtc] IS NULL");
    }
}
