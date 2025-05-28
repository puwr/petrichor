using Domain.Images;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

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
            originalImage.Property(o => o.Path)
                .HasColumnName("OriginalImagePath");

            originalImage.Property(o => o.Width)
                .HasColumnName("OriginalImageWidth");

            originalImage.Property(o => o.Height)
                .HasColumnName("OriginalImageHeight");
        });

        builder.OwnsOne(i => i.Thumbnail, thumbnail =>
        {
            thumbnail.Property(o => o.Path)
                .HasColumnName("ThumbnailPath");

            thumbnail.Property(o => o.Width)
                .HasColumnName("ThumbnailWidth");

            thumbnail.Property(o => o.Height)
                .HasColumnName("ThumbnailHeight");
        });

        builder
            .HasMany(i => i.Tags)
            .WithMany(t => t.Images)
            .UsingEntity("ImageTags");
    }
}