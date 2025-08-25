using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Petrichor.Services.Comments.Api.Common.Domain;

namespace Petrichor.Services.Comments.Api.Common.Persistence.Configurations;

public class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id).ValueGeneratedNever();

        builder.Property(c => c.Message).HasMaxLength(1000);

        builder.HasIndex(c => c.Id);
        builder.HasIndex(c => c.ResourceId);
    }
}