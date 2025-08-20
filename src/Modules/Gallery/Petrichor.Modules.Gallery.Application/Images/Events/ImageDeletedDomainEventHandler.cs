using Petrichor.Shared.Application.Common.Interfaces.Services.Storage;
using Petrichor.Modules.Gallery.Domain.Images.Events;
using Petrichor.Shared.Application.Common.Events;

namespace Petrichor.Modules.Gallery.Application.Images.Events;

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