using Microsoft.EntityFrameworkCore;
using Petrichor.Modules.Gallery.Domain.Images;
using Petrichor.Modules.Gallery.Domain.Tags;

namespace Petrichor.Modules.Gallery.Application.Common.Interfaces;

public interface IGalleryDbContext
{
    DbSet<Image> Images { get; }
    DbSet<Tag> Tags { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}