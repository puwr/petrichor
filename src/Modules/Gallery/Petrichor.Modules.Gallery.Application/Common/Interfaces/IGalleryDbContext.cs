using Microsoft.EntityFrameworkCore;
using Petrichor.Modules.Gallery.Domain.Images;
using Petrichor.Modules.Gallery.Domain.Tags;
using Petrichor.Shared.Infrastructure.Inbox;
using Petrichor.Shared.Infrastructure.Outbox;

namespace Petrichor.Modules.Gallery.Application.Common.Interfaces;

public interface IGalleryDbContext : IInboxDbContext, IOutboxDbContext
{
    DbSet<Image> Images { get; }
    DbSet<Tag> Tags { get; }

    new Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}