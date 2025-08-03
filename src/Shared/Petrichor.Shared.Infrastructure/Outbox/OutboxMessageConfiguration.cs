using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Petrichor.Shared.Infrastructure.Outbox;

public class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.HasKey(o => o.Id);

        builder.Property(o => o.Content).HasMaxLength(2000).HasColumnType("jsonb");
    }

}
