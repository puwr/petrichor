using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Petrichor.Services.Gallery.Common.Persistence;

namespace Petrichor.Services.Gallery.Features.GetImage;

public class GetImageQueryHandler(GalleryDbContext dbContext)
    : IRequestHandler<GetImageQuery, ErrorOr<GetImageResponse>>
{
    public async Task<ErrorOr<GetImageResponse>> Handle(
        GetImageQuery request,
        CancellationToken cancellationToken)
    {
        var image = await dbContext.Images
            .AsNoTracking()
            .Include(i => i.Tags)
            .FirstOrDefaultAsync(i => i.Id == request.ImageId, cancellationToken: cancellationToken);

        if (image is null)
        {
            return Error.NotFound("Image not found.");
        }

        return new GetImageResponse(
            Id: image.Id,
            Url: image.OriginalImage.Path,
            Width: image.OriginalImage.Width,
            Height: image.OriginalImage.Height,
            UploaderId: image.UploaderId,
            Tags: [.. image.Tags.Select(tag => new TagResponse(tag.Id, tag.Name))],
            UploadedAt: image.UploadedDateTime);
    }
}