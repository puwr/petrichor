using Microsoft.EntityFrameworkCore;
using Petrichor.Modules.Gallery.Application.Common.Interfaces;
using Petrichor.Modules.Gallery.Domain.Images;
using Petrichor.Modules.Gallery.Domain.Tags;
using Petrichor.Modules.Gallery.Infrastructure.Persistence.Configurations;
using Petrichor.Shared.Infrastructure.Outbox;

namespace Petrichor.Modules.Gallery.Infrastructure.Persistence;

public class GalleryDbContext(
    DbContextOptions<GalleryDbContext> options)
    : DbContext(options), IGalleryDbContext, IOutboxDbContext
{
    public DbSet<Image> Images { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<OutboxMessage> OutboxMessages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("gallery");

        modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration());
        modelBuilder.ApplyConfiguration(new ImageConfiguration());
        modelBuilder.ApplyConfiguration(new TagConfiguration());

        base.OnModelCreating(modelBuilder);
    }
}