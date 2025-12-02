using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Petrichor.Services.Gallery.Common.Domain;

namespace Petrichor.Services.Gallery.Common.Persistence.Configurations;

public class TagConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.HasKey(t => t.Id);
        builder
            .Property(t => t.Id)
            .ValueGeneratedNever();

        builder
            .HasIndex(t => t.Name)
            .IsUnique();

        builder
            .HasMany(t => t.Images)
            .WithMany(i => i.Tags);
    }
}