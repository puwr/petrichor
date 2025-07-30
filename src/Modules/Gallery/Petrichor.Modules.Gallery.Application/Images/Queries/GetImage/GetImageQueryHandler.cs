using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Petrichor.Modules.Gallery.Application.Common.Interfaces;
using Petrichor.Modules.Gallery.Application.Common.Mappings;
using Petrichor.Modules.Gallery.Contracts.Images;

namespace Petrichor.Modules.Gallery.Application.Images.Queries.GetImage;

public class GetImageQueryHandler(IGalleryDbContext dbContext)
    : IRequestHandler<GetImageQuery, ErrorOr<ImageResponse>>
{
    public async Task<ErrorOr<ImageResponse>> Handle(
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

        return image.ToResponse();
    }
}