using ErrorOr;
using Microsoft.EntityFrameworkCore;
using Petrichor.Services.Gallery.Common.Domain.Images.Events;
using Petrichor.Services.Gallery.Common.Persistence;
using Petrichor.Services.Gallery.IntegrationMessages;
using Wolverine;
using ZiggyCreatures.Caching.Fusion;

namespace Petrichor.Services.Gallery.Features.DeleteImage;

public static class DeleteImageCommandHandler
{
    public static async Task<ErrorOr<Deleted>> Handle(
        DeleteImageCommand command,
        GalleryDbContext dbContext,
        IMessageBus bus,
        IFusionCache cache,
        CancellationToken cancellationToken)
    {
        var image = await dbContext.Images
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == command.ImageId,
                cancellationToken: cancellationToken);

        if (image is null)
        {
            return Result.Deleted;
        }

        dbContext.Images.Remove(image);

        await bus.PublishAsync(new ImageDeletedDomainEvent(
            image.OriginalImage.Path,
            image.Thumbnail.Path));

        await bus.PublishAsync(new ImageDeletedIntegrationEvent(image.Id));

        await dbContext.SaveChangesAsync(cancellationToken);

        await cache.RemoveAsync($"image:{image.Id}", token: cancellationToken);
        await cache.RemoveByTagAsync("images", token: cancellationToken);

        return Result.Deleted;
    }
}