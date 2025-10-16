using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Petrichor.Shared.Pagination;
using Petrichor.Shared.Extensions;
using Petrichor.Services.Gallery.Common.Persistence;
using Petrichor.Services.Gallery.Common.Utilities;
using ZiggyCreatures.Caching.Fusion;

namespace Petrichor.Services.Gallery.Features.GetImages;

public class GetImagesQueryHandler(IServiceScopeFactory scopeFactory, IFusionCache cache)
    : IRequestHandler<GetImagesQuery, ErrorOr<PagedResponse<GetImagesResponse>>>
{
    public async Task<ErrorOr<PagedResponse<GetImagesResponse>>> Handle(
        GetImagesQuery request,
        CancellationToken cancellationToken)
    {
        var normalizedTags = TagHelpers.Normalize(request.Tags);
        var tagsHash = TagHelpers.Hash(normalizedTags);

        var response = await cache.GetOrSetAsync(
            $"images:page-{request.Pagination.PageNumber}:tags-{tagsHash}",
            async _ =>
            {
                await using var scope = scopeFactory.CreateAsyncScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<GalleryDbContext>();

                var query = dbContext.Images.AsQueryable();

                query = query.WhereIf(
                    condition: normalizedTags?.Count > 0,
                    predicate: image => normalizedTags!
                        .All(nt => image.Tags.Any(t => t.Name.Contains(nt)))
                );

                var images = await query
                    .AsNoTracking()
                    .OrderByDescending(i => i.UploadedDateTime)
                    .Select(image => GetImagesResponse.From(image))
                    .ToPagedResponseAsync(request.Pagination, cancellationToken: cancellationToken);

                return images;
            },
            tags: ["images"],
            token: cancellationToken
        );

        return response;
    }
}