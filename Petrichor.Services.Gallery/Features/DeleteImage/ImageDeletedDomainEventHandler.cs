using Petrichor.Services.Gallery.Common.Domain.Images.Events;
using Petrichor.Shared.DomainEvents;
using Petrichor.Shared.Services.Storage;

namespace Petrichor.Services.Gallery.Features.DeleteImage;

public class ImageDeletedDomainEventHandler(IFileStorage fileStorage)
    : IDomainEventHandler<ImageDeletedDomainEvent>
{
    public async Task Handle(
        ImageDeletedDomainEvent @event,
        CancellationToken cancellationToken)
    {
        await fileStorage.DeleteFileAsync(@event.ImagePath);
        await fileStorage.DeleteFileAsync(@event.ThumbnailPath);
    }
}