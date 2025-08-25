using Microsoft.EntityFrameworkCore;
using Petrichor.Modules.Gallery.Domain.Images;
using Petrichor.Modules.Gallery.Domain.Tags;
using Petrichor.Shared.Inbox;
using Petrichor.Shared.Outbox;

namespace Petrichor.Modules.Gallery.Application.Common.Interfaces;

public interface IGalleryDbContext : IInboxDbContext, IOutboxDbContext
{
    DbSet<Image> Images { get; }
    DbSet<Tag> Tags { get; }

    new Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}