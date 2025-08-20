using Microsoft.EntityFrameworkCore;
using Petrichor.Modules.Gallery.Application.Common.Interfaces;
using Petrichor.Modules.Users.IntegrationEvents;
using Petrichor.Shared.Application.Common.Events;

namespace Petrichor.Modules.Gallery.Application.IntegrationEventHandlers;

public class UserDeletedIntegrationEventHandler(IGalleryDbContext dbContext)
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
                image.DeleteImage();
                dbContext.Images.Remove(image);
            }

            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}