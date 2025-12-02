using Microsoft.EntityFrameworkCore;
using Petrichor.Services.Gallery.Common.Domain;
using Petrichor.Services.Gallery.Common.Domain.Images;
using Petrichor.Services.Gallery.Common.Persistence.Configurations;
using Wolverine.EntityFrameworkCore;

namespace Petrichor.Services.Gallery.Common.Persistence;

public class GalleryDbContext(DbContextOptions<GalleryDbContext> options)
    : DbContext(options)
{
    public DbSet<Image> Images { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<UserSnapshot> UserSnapshots { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("gallery");

        modelBuilder.MapWolverineEnvelopeStorage();

        modelBuilder.ApplyConfiguration(new ImageConfiguration());
        modelBuilder.ApplyConfiguration(new TagConfiguration());
        modelBuilder.ApplyConfiguration(new UserSnapshotConfiguration());

        base.OnModelCreating(modelBuilder);
    }
}