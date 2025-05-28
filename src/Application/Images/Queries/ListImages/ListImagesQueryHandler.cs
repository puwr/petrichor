using Application.Common.Interfaces;
using Application.Common.Mappings;
using Contracts.Images;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Images.Queries.ListImages;

public class ListImagesQueryHandler(IPetrichorDbContext dbContext) 
    : IRequestHandler<ListImagesQuery, ErrorOr<List<ListImagesResponse>>>
{
    public async Task<ErrorOr<List<ListImagesResponse>>> Handle(
        ListImagesQuery request,
        CancellationToken cancellationToken)
    {
        var images = await dbContext.Images
            .AsNoTracking()
            .OrderByDescending(i => i.UploadedDateTime)
            .Select(i => i.ToListResponse())
            .ToListAsync(cancellationToken: cancellationToken);
        
        return images;
    }
}