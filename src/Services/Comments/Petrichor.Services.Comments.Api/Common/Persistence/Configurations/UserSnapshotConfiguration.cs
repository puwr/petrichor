using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Petrichor.Services.Comments.Api.Common.Domain;

namespace Petrichor.Services.Comments.Api.Common.Persistence.Configurations;

public class UserSnapshotConfiguration : IEntityTypeConfiguration<UserSnapshot>
{
    public void Configure(EntityTypeBuilder<UserSnapshot> builder)
    {
        builder.HasKey(us => us.UserId);

        builder.Property(us => us.UserId).ValueGeneratedNever();

        builder.HasIndex(us => us.UserId);
    }
}