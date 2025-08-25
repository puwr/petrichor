using Microsoft.EntityFrameworkCore;
using Petrichor.Modules.Gallery.Application.Common.Interfaces;
using Petrichor.Modules.Gallery.Domain.Images.Events;
using Petrichor.Modules.Users.IntegrationMessages;
using Petrichor.Shared.Events;
using Petrichor.Shared.Outbox;

namespace Petrichor.Modules.Gallery.Application.IntegrationEventHandlers;

public class UserDeletedIntegrationEventHandler(
    IGalleryDbContext dbContext,
    EventPublisher<IGalleryDbContext> eventPublisher)
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
        }
    }
}