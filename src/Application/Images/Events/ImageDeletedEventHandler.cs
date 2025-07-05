using Application.Common.Interfaces.Services.Storage;
using Domain.Images.Events;
using Mediator;

namespace Application.Images.Events;

public class ImageDeletedEventHandler(
    IFileStorage fileStorage)
    : INotificationHandler<ImageDeletedEvent>
{
    public async ValueTask Handle(ImageDeletedEvent notification, CancellationToken cancellationToken)
    {
        await fileStorage.DeleteFileAsync(notification.ImagePath);
        await fileStorage.DeleteFileAsync(notification.ThumbnnailPath);
    }
}
