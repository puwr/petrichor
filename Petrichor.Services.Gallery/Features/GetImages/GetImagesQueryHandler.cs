using Microsoft.EntityFrameworkCore;
using Petrichor.Shared.Extensions;
using Petrichor.Services.Gallery.Common.Persistence;
using Petrichor.Services.Gallery.Common.Utilities;
using ZiggyCreatures.Caching.Fusion;
using Petrichor.Shared.Pagination;

namespace Petrichor.Services.Gallery.Features.GetImages;

public static class GetImagesQueryHandler
{
    public static async Task<PagedResponse<GetImagesResponse>> Handle(
        GetImagesQuery query,
        IDbContextFactory<GalleryDbContext> dbContextFactory,
        IFusionCache cache,
        CancellationToken cancellationToken)
    {
        var normalizedTags = TagHelpers.Normalize(query.Tags);
        var tagsHash = TagHelpers.Hash(normalizedTags);
        var uploader = query.Uploader?.Trim().ToLowerInvariant();

        var response = await cache.GetOrSetAsync(
            $"images:page-{query.Pagination.PageNumber}:tags-{tagsHash}:uploader-{query.Uploader ?? "all"}",
            async _ =>
            {
                var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);

                var imagesQuery = dbContext.Images.AsNoTracking().AsQueryable();

                imagesQuery = imagesQuery.WhereIf(
                    condition: normalizedTags?.Count > 0,
                    predicate: image => normalizedTags!
                        .All(nt => image.Tags.Any(t => t.Name.Contains(nt)))
                );

                imagesQuery = imagesQuery.WhereIf(
                    condition: !string.IsNullOrWhiteSpace(uploader),
                    predicate: image => dbContext.UserSnapshots
                        .Any(us => us.UserName == uploader && us.UserId == image.UploaderId)
                );

                var response = await imagesQuery
                    .AsNoTracking()
                    .OrderByDescending(i => i.UploadedDateTime)
                    .Select(image => GetImagesResponse.From(image))
                    .ToPagedResponseAsync(query.Pagination, cancellationToken);

                return response;
            },
            tags: ["images"],
            token: cancellationToken
        );

        return response;
    }
}