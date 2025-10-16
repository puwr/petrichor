using Microsoft.EntityFrameworkCore;
using Petrichor.Services.Gallery.Common.Domain.Images.Events;
using Petrichor.Services.Gallery.Common.Persistence;
using Petrichor.Services.Users.IntegrationMessages;
using Petrichor.Shared.Events;
using Petrichor.Shared.Outbox;
using ZiggyCreatures.Caching.Fusion;

namespace Petrichor.Services.Gallery.IntegrationMessageHandlers;

public class UserDeletedIntegrationEventHandler(
    GalleryDbContext dbContext,
    EventPublisher<GalleryDbContext> eventPublisher,
    IFusionCache cache)
    : IIntegrationEventHandler<UserDeletedIntegrationEvent>
{
    public async Task Handle(UserDeletedIntegrationEvent @event, CancellationToken cancellationToken)
    {
        if (@event.DeleteUploadedImages)
        {
            var images = await dbContext.Images
                .Where(i => i.UploaderId == @event.UserId)
                .ToListAsync(cancellationToken);

            foreach (var image in images)
            {
                eventPublisher.Publish(new ImageDeletedDomainEvent(
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