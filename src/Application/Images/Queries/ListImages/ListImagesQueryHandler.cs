using Application.Common.Extensions;
using Application.Common.Interfaces;
using Application.Common.Mappings;
using Contracts.Images;
using Contracts.Pagination;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Images.Queries.ListImages;

public class ListImagesQueryHandler(IPetrichorDbContext dbContext)
    : IRequestHandler<ListImagesQuery, ErrorOr<PagedResponse<ListImagesResponse>>>
{
    public async Task<ErrorOr<PagedResponse<ListImagesResponse>>> Handle(
        ListImagesQuery request,
        CancellationToken cancellationToken)
    {
        var query = dbContext.Images.AsQueryable();

        var images = await query
            .AsNoTracking()
            .OrderByDescending(i => i.UploadedDateTime)
            .Select(i => i.ToListResponse())
            .ToPagedResponseAsync(request.Pagination, cancellationToken: cancellationToken);

        return images;
    }
}