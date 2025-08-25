using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Petrichor.Shared.Outbox;

public class OutboxMessageConsumerConfiguration : IEntityTypeConfiguration<OutboxMessageConsumer>
{
    public void Configure(EntityTypeBuilder<OutboxMessageConsumer> builder)
    {
        builder.HasKey(omc => new { omc.OutboxMessageId, omc.Name });

        builder.Property(omc => omc.Name).HasMaxLength(600);
    }

}
