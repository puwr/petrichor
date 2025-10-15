using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Petrichor.Shared.Pagination;
using Petrichor.Shared.Extensions;
using Petrichor.Services.Gallery.Common.Persistence;
using Petrichor.Services.Gallery.Common.Utilities;

namespace Petrichor.Services.Gallery.Features.GetImages;

public class GetImagesQueryHandler(GalleryDbContext dbContext)
    : IRequestHandler<GetImagesQuery, ErrorOr<PagedResponse<GetImagesResponse>>>
{
    public async Task<ErrorOr<PagedResponse<GetImagesResponse>>> Handle(
        GetImagesQuery request,
        CancellationToken cancellationToken)
    {
        var query = dbContext.Images.AsQueryable();

        var normalizedTags = TagHelpers.Normalize(request.Tags);

        query = query.WhereIf(
            condition: normalizedTags?.Count > 0,
            predicate: image => normalizedTags!
                .All(nt => image.Tags.Any(t => t.Name.Contains(nt)))
        );

        var images = await query
            .AsNoTracking()
            .OrderByDescending(i => i.UploadedDateTime)
            .Select(i => new GetImagesResponse(
                i.Id,
                i.Thumbnail.Path,
                i.Thumbnail.Width,
                i.Thumbnail.Height))
            .ToPagedResponseAsync(request.Pagination, cancellationToken: cancellationToken);

        return images;
    }
}