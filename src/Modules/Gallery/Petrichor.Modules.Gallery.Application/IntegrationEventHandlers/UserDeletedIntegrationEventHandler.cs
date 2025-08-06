using MediatR;
using Microsoft.EntityFrameworkCore;
using Petrichor.Modules.Gallery.Application.Common.Interfaces;
using Petrichor.Modules.Users.IntegrationEvents;

namespace Petrichor.Modules.Gallery.Application.IntegrationEventHandlers;

public class UserDeletedIntegrationEventHandler(IGalleryDbContext dbContext)
    : INotificationHandler<UserDeletedIntegrationEvent>
{
    public async Task Handle(UserDeletedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        if (notification.DeleteUploadedImages)
        {
            var images = await dbContext.Images
                .Where(i => i.UploaderId == notification.UserId)
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