using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Petrichor.Shared.Inbox;

public class InboxMessageConsumerConfiguration : IEntityTypeConfiguration<InboxMessageConsumer>
{
    public void Configure(EntityTypeBuilder<InboxMessageConsumer> builder)
    {
        builder.HasKey(imc => new { imc.InboxMessageId, imc.Name });

        builder.Property(imc => imc.Name).HasMaxLength(600);
    }
}