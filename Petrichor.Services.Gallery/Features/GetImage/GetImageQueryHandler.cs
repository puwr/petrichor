using ErrorOr;
using Microsoft.EntityFrameworkCore;
using Petrichor.Services.Gallery.Common.Persistence;
using ZiggyCreatures.Caching.Fusion;

namespace Petrichor.Services.Gallery.Features.GetImage;

public static class GetImageQueryHandler
{
    public static async Task<ErrorOr<GetImageResponse>> Handle(
        GetImageQuery query,
        IDbContextFactory<GalleryDbContext> dbContextFactory,
        IFusionCache cache,
        CancellationToken cancellationToken)
    {
        var response = await cache.GetOrSetAsync(
            $"image:{query.ImageId}",
            async _ =>
            {
                var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);

                var data = await dbContext.Images
                    .AsNoTracking()
                    .Where(i => i.Id == query.ImageId)
                    .Include(i => i.Tags)
                    .Select(i => new
                    {
                        Image = i,
                        UserSnapshot = dbContext.UserSnapshots
                            .FirstOrDefault(us => us.UserId == i.UploaderId)
                    })
                    .FirstOrDefaultAsync(cancellationToken);

                if (data?.Image is null) return null;

                return GetImageResponse.From(data.Image, data.UserSnapshot);
            },
            token: cancellationToken
        );

        return response is not null
            ? response
            : Error.NotFound(description: "Image not found.");
    }
}