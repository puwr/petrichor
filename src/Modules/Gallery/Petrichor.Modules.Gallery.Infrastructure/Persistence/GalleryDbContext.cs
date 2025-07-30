using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Petrichor.Shared.Domain.Common;
using Petrichor.Modules.Gallery.Application.Common.Interfaces;
using Petrichor.Modules.Gallery.Domain.Images;
using Petrichor.Modules.Gallery.Domain.Tags;
using Petrichor.Modules.Gallery.Infrastructure.Persistence.Configurations;

namespace Petrichor.Modules.Gallery.Infrastructure.Persistence;

public class GalleryDbContext(
    DbContextOptions<GalleryDbContext> options,
    IHttpContextAccessor httpContextAccessor,
    IPublisher publisher)
    : DbContext(options), IGalleryDbContext
{
    public DbSet<Image> Images { get; set; }
    public DbSet<Tag> Tags { get; set; }

    public override async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        var domainEvents = ChangeTracker.Entries<Entity>()
            .Select(entry => entry.Entity.PopDomainEvents())
            .SelectMany(e => e)
            .ToList();

        if (IsUserWaitingOnline())
        {
            AddDomainEventsToOfflineProcessingQueue(domainEvents);
        }
        else
        {
            await PublishDomainEvents(publisher, domainEvents);
        }

        return await base.SaveChangesAsync(cancellationToken);
    }

    private bool IsUserWaitingOnline() => httpContextAccessor?.HttpContext is not null;

    private void AddDomainEventsToOfflineProcessingQueue(List<IDomainEvent> domainEvents)
    {
        var domainEventsQueue = httpContextAccessor.HttpContext!.Items
            .TryGetValue("DomainEventsQueue", out var value)
                && value is Queue<IDomainEvent> existingDomainEvents
            ? existingDomainEvents
            : new Queue<IDomainEvent>();

        domainEvents.ForEach(domainEventsQueue.Enqueue);

        httpContextAccessor.HttpContext.Items["DomainEventsQueue"] = domainEventsQueue;
    }

    private static async Task PublishDomainEvents(
        IPublisher publisher,
        List<IDomainEvent> domainEvents)
    {
        foreach (var domainEvent in domainEvents)
        {
            await publisher.Publish(domainEvent);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("gallery");

        modelBuilder.ApplyConfiguration(new ImageConfiguration());
        modelBuilder.ApplyConfiguration(new TagConfiguration());

        base.OnModelCreating(modelBuilder);
    }
}