using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Petrichor.Modules.Gallery.Application.Common.Interfaces;
using Petrichor.Modules.Gallery.Domain.Images;
using Petrichor.Modules.Gallery.Domain.Tags;
using Petrichor.Modules.Gallery.Infrastructure.Persistence.Configurations;
using Petrichor.Shared.Inbox;
using Petrichor.Shared.Outbox;

namespace Petrichor.Modules.Gallery.Infrastructure.Persistence;

public class GalleryDbContext(
    DbContextOptions<GalleryDbContext> options)
    : DbContext(options), IGalleryDbContext
{
    public DbSet<Image> Images { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<InboxMessage> InboxMessages { get; set; }
    public DbSet<InboxMessageConsumer> InboxMessageConsumers { get; set; }
    public DbSet<OutboxMessage> OutboxMessages { get; set; }
    public DbSet<OutboxMessageConsumer> OutboxMessageConsumers { get; set; }

    public Task<IDbContextTransaction> BeginTransactionAsync(
        CancellationToken cancellationToken = default)
    {
        return Database.BeginTransactionAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("gallery");

        modelBuilder.ApplyConfiguration(new InboxMessageConfiguration());
        modelBuilder.ApplyConfiguration(new InboxMessageConsumerConfiguration());
        modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration());
        modelBuilder.ApplyConfiguration(new OutboxMessageConsumerConfiguration());
        modelBuilder.ApplyConfiguration(new ImageConfiguration());
        modelBuilder.ApplyConfiguration(new TagConfiguration());

        base.OnModelCreating(modelBuilder);
    }
}