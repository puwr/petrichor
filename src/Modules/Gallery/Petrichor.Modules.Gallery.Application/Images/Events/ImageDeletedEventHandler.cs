using MediatR;
using Petrichor.Shared.Application.Common.Interfaces.Services.Storage;
using Petrichor.Modules.Gallery.Domain.Images.Events;

namespace Petrichor.Modules.Gallery.Application.Images.Events;

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
