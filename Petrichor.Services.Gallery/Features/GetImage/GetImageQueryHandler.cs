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
        var response = await cache.GetOrSetAsync<ErrorOr<GetImageResponse>>(
            $"image:{request.ImageId}",
            async _ =>
            {
                await using var scope = scopeFactory.CreateAsyncScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<GalleryDbContext>();

                var data = await dbContext.Images
                    .AsNoTracking()
                    .Where(i => i.Id == request.ImageId)
                    .Include(i => i.Tags)
                    .Select(i => new
                    {
                        Image = i,
                        UserSnapshot = dbContext.UserSnapshots
                            .FirstOrDefault(us => us.UserId == i.UploaderId)
                    })
                    .FirstOrDefaultAsync(cancellationToken);

                if (data?.Image is null) return Error.NotFound(description: "Image not found.");

                return GetImageResponse.From(data.Image, data.UserSnapshot);
            },
            token: cancellationToken
        );

        return response;
    }
}