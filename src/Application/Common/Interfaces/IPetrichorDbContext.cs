using Domain.Images;
using Domain.Tags;
using Microsoft.EntityFrameworkCore;

namespace Application.Common.Interfaces;

public interface IPetrichorDbContext
{
    DbSet<Image> Images { get; }
    DbSet<Tag> Tags { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}