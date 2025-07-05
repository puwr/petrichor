using Application.Common.Interfaces;
using Application.Common.Mappings;
using Contracts.Images;
using ErrorOr;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.Images.Queries.GetImage;

public class GetImageQueryHandler(IPetrichorDbContext dbContext)
    : IRequestHandler<GetImageQuery, ErrorOr<ImageResponse>>
{
    public async ValueTask<ErrorOr<ImageResponse>> Handle(
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