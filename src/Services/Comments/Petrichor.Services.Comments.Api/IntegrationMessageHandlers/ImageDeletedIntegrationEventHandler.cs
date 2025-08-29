using Microsoft.EntityFrameworkCore;
using Petrichor.Modules.Gallery.IntegrationMessages;
using Petrichor.Services.Comments.Api.Common.Persistence;
using Petrichor.Shared.Events;

namespace Petrichor.Services.Comments.Api.IntegrationMessageHandlers;

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