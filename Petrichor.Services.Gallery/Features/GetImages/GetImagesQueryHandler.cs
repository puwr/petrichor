using ErrorOr;
using Microsoft.EntityFrameworkCore;
using Petrichor.Shared.Pagination;
using Petrichor.Shared.Extensions;
using Petrichor.Services.Gallery.Common.Persistence;
using Petrichor.Services.Gallery.Common.Utilities;
using ZiggyCreatures.Caching.Fusion;

namespace Petrichor.Services.Gallery.Features.GetImages;

public static class GetImagesQueryHandler
{
    public static async Task<ErrorOr<PagedResponse<GetImagesResponse>>> Handle(
        GetImagesQuery request,
        IDbContextFactory<GalleryDbContext> dbContextFactory,
        IFusionCache cache,
        CancellationToken cancellationToken)
    {
        var normalizedTags = TagHelpers.Normalize(request.Tags);
        var tagsHash = TagHelpers.Hash(normalizedTags);

        var response = await cache.GetOrSetAsync(
            $"images:page-{request.Pagination.PageNumber}:tags-{tagsHash}:uploader-{request.Uploader ?? "all"}",
            async _ =>
            {
                var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);

                var query = dbContext.Images.AsNoTracking().AsQueryable();

                query = query.WhereIf(
                    condition: normalizedTags?.Count > 0,
                    predicate: image => normalizedTags!
                        .All(nt => image.Tags.Any(t => t.Name.Contains(nt)))
                );

                query = query.WhereIf(
                    condition: !string.IsNullOrWhiteSpace(request.Uploader),
                    predicate: image => dbContext.UserSnapshots
                        .Any(us => us.UserName == request.Uploader && us.UserId == image.UploaderId)
                );

                var images = await query
                    .AsNoTracking()
                    .OrderByDescending(i => i.UploadedDateTime)
                    .Select(image => GetImagesResponse.From(image))
                    .ToPagedResponseAsync(request.Pagination, cancellationToken);

                return images;
            },
            tags: ["images"],
            token: cancellationToken
        );

        return response;
    }
}