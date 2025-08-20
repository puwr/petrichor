using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Petrichor.Modules.Gallery.Domain.Images;

namespace Petrichor.Modules.Gallery.Infrastructure.Persistence.Configurations;

public class ImageConfiguration : IEntityTypeConfiguration<Image>
{
    public void Configure(EntityTypeBuilder<Image> builder)
    {
        builder.HasKey(i => i.Id);

        builder
            .Property(i => i.Id)
            .ValueGeneratedNever();

        builder.OwnsOne(i => i.OriginalImage, originalImage =>
        {
            originalImage.Property(o => o.Path);

            originalImage.Property(o => o.Width);

            originalImage.Property(o => o.Height);
        });

        builder.OwnsOne(i => i.Thumbnail, thumbnail =>
        {
            thumbnail.Property(o => o.Path);

            thumbnail.Property(o => o.Width);

            thumbnail.Property(o => o.Height);
        });

        builder
            .HasMany(i => i.Tags)
            .WithMany(t => t.Images)
            .UsingEntity("image_tags");
    }
}