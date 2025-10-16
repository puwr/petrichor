using ErrorOr;
using MassTransit.Initializers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Petrichor.Services.Gallery.Common.Persistence;
using ZiggyCreatures.Caching.Fusion;

namespace Petrichor.Services.Gallery.Features.GetImage;

public class GetImageQueryHandler(IServiceScopeFactory scopeFactory, IFusionCache cache)
    : IRequestHandler<GetImageQuery, ErrorOr<GetImageResponse>>
{
    public async Task<ErrorOr<GetImageResponse>> Handle(
        GetImageQuery request,
        CancellationToken cancellationToken)
    {
        var response = await cache.GetOrSetAsync(
            $"image:{request.ImageId}",
            async _ =>
            {
                await using var scope = scopeFactory.CreateAsyncScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<GalleryDbContext>();

                var image = await dbContext.Images
                    .AsNoTracking()
                    .Include(i => i.Tags)
                    .FirstOrDefaultAsync(i => i.Id == request.ImageId, cancellationToken);

                return image is null ? null : GetImageResponse.From(image);
            },
            token: cancellationToken
        );

        if (response is null)
        {
            return Error.NotFound(description: "Image not found.");
        }

        return response;
    }
}