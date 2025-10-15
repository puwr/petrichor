using Microsoft.EntityFrameworkCore;
using Petrichor.Services.Comments.Common.Persistence;
using Petrichor.Services.Gallery.IntegrationMessages;
using Petrichor.Shared.Events;

namespace Petrichor.Services.Comments.IntegrationMessageHandlers;

public class ImageDeletedIntegrationEventHandler(CommentsDbContext dbContext)
    : IIntegrationEventHandler<ImageDeletedIntegrationEvent>
{
    public async Task Handle(
        ImageDeletedIntegrationEvent @event,
        CancellationToken cancellationToken = default)
    {
        await dbContext.Comments
            .Where(c => c.ResourceId == @event.ImageId)
            .ExecuteDeleteAsync(cancellationToken);
    }
}