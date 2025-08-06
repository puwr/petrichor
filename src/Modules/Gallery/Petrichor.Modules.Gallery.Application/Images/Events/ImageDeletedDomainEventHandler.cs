using MediatR;
using Petrichor.Shared.Application.Common.Interfaces.Services.Storage;
using Petrichor.Modules.Gallery.Domain.Images.Events;

namespace Petrichor.Modules.Gallery.Application.Images.Events;

public class ImageDeletedDomainEventHandler(
    IFileStorage fileStorage)
    : INotificationHandler<ImageDeletedDomainEvent>
{
    public async Task Handle(ImageDeletedDomainEvent notification, CancellationToken cancellationToken)
    {
        await fileStorage.DeleteFileAsync(notification.ImagePath);
        await fileStorage.DeleteFileAsync(notification.ThumbnailPath);
    }
}