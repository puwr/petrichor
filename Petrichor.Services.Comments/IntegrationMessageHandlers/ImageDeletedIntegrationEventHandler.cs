using Microsoft.EntityFrameworkCore;
using Petrichor.Services.Comments.Common.Persistence;
using Petrichor.Services.Gallery.IntegrationMessages;
using ZiggyCreatures.Caching.Fusion;

namespace Petrichor.Services.Comments.IntegrationMessageHandlers;

public static class ImageDeletedIntegrationEventHandler
{
    public static async Task Handle(
        ImageDeletedIntegrationEvent @event,
        CommentsDbContext dbContext,
        IFusionCache cache,
        CancellationToken cancellationToken)
    {
        await dbContext.Comments
            .Where(c => c.ResourceId == @event.ImageId)
            .ExecuteDeleteAsync(cancellationToken);

        await cache.RemoveByTagAsync(
            $"comments:{@event.ImageId}",
            token: cancellationToken);
    }
}