using Domain.Tags;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Tags.Persistence;

public class TagConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
            .ValueGeneratedNever();

        builder.HasMany(t => t.Images).WithMany(i => i.Tags).UsingEntity(j => j.ToTable("ImageTags"));
    }
}