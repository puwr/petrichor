using Application.Common.Interfaces.Services.Storage;
using Domain.Images.Events;
using MediatR;

namespace Application.Images.Events;

public class ImageDeletedEventHandler(
    IFileStorage fileStorage) 
    : INotificationHandler<ImageDeletedEvent>
{
    public async Task Handle(ImageDeletedEvent notification, CancellationToken cancellationToken)
    {
        await fileStorage.DeleteFileAsync(notification.ImagePath);
        await fileStorage.DeleteFileAsync(notification.ThumbnnailPath);
    }
}
