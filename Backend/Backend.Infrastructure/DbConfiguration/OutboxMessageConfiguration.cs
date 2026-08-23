using Backend.Infrastructure.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Infrastructure.DbConfiguration;

public class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.HasKey(message => message.Id);

        builder
            .Property(message => message.Type)
            .HasMaxLength(200);

        builder
            .Property(message => message.Content)
            .HasColumnType("jsonb");

        builder
            .Property(message => message.Error)
            .HasMaxLength(4000);

        builder.HasIndex(message => new
        {
            message.ProcessedAtUtc,
            message.FailedAtUtc,
            message.NextAttemptAtUtc,
        });
    }
}
