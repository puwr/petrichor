using Microsoft.EntityFrameworkCore;
using Petrichor.Services.Comments.Common.Persistence;
using Petrichor.Services.Gallery.IntegrationMessages;
using Petrichor.Shared.Events;
using ZiggyCreatures.Caching.Fusion;

namespace Petrichor.Services.Comments.IntegrationMessageHandlers;

public class ImageDeletedIntegrationEventHandler(CommentsDbContext dbContext, IFusionCache cache)
    : IIntegrationEventHandler<ImageDeletedIntegrationEvent>
{
    public async Task Handle(
        ImageDeletedIntegrationEvent @event,
        CancellationToken cancellationToken = default)
    {
        await dbContext.Comments
            .Where(c => c.ResourceId == @event.ImageId)
            .ExecuteDeleteAsync(cancellationToken);

        await cache.RemoveByTagAsync(
            $"comments:{@event.ImageId}",
            token: cancellationToken);
    }
}