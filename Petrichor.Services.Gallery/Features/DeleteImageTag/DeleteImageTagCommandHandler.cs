using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Petrichor.Services.Gallery.Common.Persistence;
using ZiggyCreatures.Caching.Fusion;

namespace Petrichor.Services.Gallery.Features.DeleteImageTag;

public class DeleteImageTagCommandHandler(
    GalleryDbContext dbContext,
    IFusionCache cache)
    : IRequestHandler<DeleteImageTagCommand, ErrorOr<Deleted>>
{
    public async Task<ErrorOr<Deleted>> Handle(
        DeleteImageTagCommand command,
        CancellationToken cancellationToken)
    {
        var image = await dbContext.Images
            .Include(i => i.Tags)
            .FirstOrDefaultAsync(i => i.Id == command.ImageId,
                cancellationToken: cancellationToken);

        if (image is null)
            return Result.Deleted;

        image.RemoveTag(command.TagId);
        await dbContext.SaveChangesAsync(cancellationToken);

        await cache.ExpireAsync($"image:{image.Id}", token: cancellationToken);

        return Result.Deleted;
    }
}