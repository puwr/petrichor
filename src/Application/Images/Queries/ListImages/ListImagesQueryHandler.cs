using Application.Common.Extensions;
using Application.Common.Interfaces;
using Application.Common.Mappings;
using Application.Common.Utilities;
using Contracts.Images;
using Contracts.Pagination;
using ErrorOr;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.Images.Queries.ListImages;

public class ListImagesQueryHandler(IPetrichorDbContext dbContext)
    : IRequestHandler<ListImagesQuery, ErrorOr<PagedResponse<ListImagesResponse>>>
{
    public async ValueTask<ErrorOr<PagedResponse<ListImagesResponse>>> Handle(
        ListImagesQuery request,
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