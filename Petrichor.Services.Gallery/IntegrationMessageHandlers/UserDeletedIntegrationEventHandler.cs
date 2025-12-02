using Microsoft.EntityFrameworkCore;
using Petrichor.Services.Gallery.Common.Domain.Images.Events;
using Petrichor.Services.Gallery.Common.Persistence;
using Petrichor.Services.Users.IntegrationMessages;
using Wolverine;
using ZiggyCreatures.Caching.Fusion;

namespace Petrichor.Services.Gallery.IntegrationMessageHandlers;

public static class UserDeletedIntegrationEventHandler
{
    public static async Task Handle(
        UserDeletedIntegrationEvent @event,
        GalleryDbContext dbContext,
        IMessageBus bus,
        IFusionCache cache,
        CancellationToken cancellationToken)
    {
        await dbContext.UserSnapshots
            .Where(c => c.UserId == @event.UserId)
            .ExecuteDeleteAsync(cancellationToken);

        if (@event.DeleteUploadedImages)
        {
            var images = await dbContext.Images
                .Where(i => i.UploaderId == @event.UserId)
                .ToListAsync(cancellationToken);

            foreach (var image in images)
            {
                await bus.PublishAsync(new ImageDeletedDomainEvent(
                    image.OriginalImage.Path,
                    image.Thumbnail.Path));

                dbContext.Images.Remove(image);
            }

            await dbContext.SaveChangesAsync(cancellationToken);

            foreach (var image in images)
            {
                await cache.RemoveAsync($"image:{image.Id}", token: cancellationToken);
            }

            await cache.RemoveByTagAsync("images", token: cancellationToken);
        }
    }
}