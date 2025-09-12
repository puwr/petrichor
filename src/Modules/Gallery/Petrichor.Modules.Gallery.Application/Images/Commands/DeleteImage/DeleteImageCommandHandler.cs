using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Petrichor.Modules.Gallery.Application.Common.Interfaces;
using Petrichor.Modules.Gallery.Domain.Images.Events;
using Petrichor.Modules.Gallery.IntegrationMessages;
using Petrichor.Shared.Outbox;

namespace Petrichor.Modules.Gallery.Application.Images.Commands.DeleteImage;

public class DeleteImageCommandHandler(
    IGalleryDbContext dbContext,
    EventPublisher<IGalleryDbContext> eventPublisher)
    : IRequestHandler<DeleteImageCommand, ErrorOr<Deleted>>
{
    public async Task<ErrorOr<Deleted>> Handle(
        DeleteImageCommand command,
        CancellationToken cancellationToken)
    {
        var image = await dbContext.Images
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == command.ImageId,
                cancellationToken: cancellationToken);

        if (image is null)
        {
            return Result.Deleted;
        }

        eventPublisher.Publish(new ImageDeletedDomainEvent(
            image.OriginalImage.Path,
            image.Thumbnail.Path));

        eventPublisher.Publish(new ImageDeletedIntegrationEvent(image.Id));

        dbContext.Images.Remove(image);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Deleted;
    }
}