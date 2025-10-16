using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Petrichor.Services.Gallery.Common.Domain.Images.Events;
using Petrichor.Services.Gallery.Common.Persistence;
using Petrichor.Services.Gallery.IntegrationMessages;
using Petrichor.Shared.Outbox;
using ZiggyCreatures.Caching.Fusion;

namespace Petrichor.Services.Gallery.Features.DeleteImage;

public class DeleteImageCommandHandler(
    GalleryDbContext dbContext,
    EventPublisher<GalleryDbContext> eventPublisher,
    IFusionCache cache)
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

        await cache.RemoveAsync($"image:{image.Id}", token: cancellationToken);
        await cache.RemoveByTagAsync("images", token: cancellationToken);

        return Result.Deleted;
    }
}