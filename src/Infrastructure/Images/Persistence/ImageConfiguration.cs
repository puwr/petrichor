using Domain.Images;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Images.Persistence;

public class ImageConfiguration : IEntityTypeConfiguration<Image>
{
    public void Configure(EntityTypeBuilder<Image> builder)
    {
        builder.HasKey(i => i.Id);

        builder.Property(i => i.Id)
            .ValueGeneratedNever();

        builder.HasMany(i => i.Tags).WithMany(t => t.Images).UsingEntity(j => j.ToTable("ImageTags"));
    }
}