using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Petrichor.Modules.Gallery.Application.Common.Interfaces;
using Petrichor.Modules.Gallery.Application.Common.Mappings;
using Petrichor.Modules.Gallery.Application.Common.Utilities;
using Petrichor.Modules.Gallery.Contracts.Images;
using Petrichor.Shared.Pagination;
using Petrichor.Shared.Extensions;

namespace Petrichor.Modules.Gallery.Application.Images.Queries.GetImages;

public class GetImagesQueryHandler(IGalleryDbContext dbContext)
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
            .Select(i => i.ToListResponse())
            .ToPagedResponseAsync(request.Pagination, cancellationToken: cancellationToken);

        return images;
    }
}