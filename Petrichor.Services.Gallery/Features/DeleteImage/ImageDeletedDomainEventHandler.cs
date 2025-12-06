using Petrichor.Services.Gallery.Common.Domain.Images.Events;
using Petrichor.Shared.Services.Storage;

namespace Petrichor.Services.Gallery.Features.DeleteImage;

public static class ImageDeletedDomainEventHandler
{
    public static async Task Handle(
        ImageDeletedDomainEvent @event,
        IFileStorage fileStorage,
        CancellationToken cancellationToken)
    {
        await fileStorage.DeleteFileAsync(@event.ImagePath, cancellationToken);
        await fileStorage.DeleteFileAsync(@event.ThumbnailPath, cancellationToken);
    }
}