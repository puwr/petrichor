using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Petrichor.Services.Gallery.Common.Persistence;

namespace Petrichor.Services.Gallery.Features.DeleteImageTag;

public class DeleteImageTagCommandHandler(
    GalleryDbContext dbContext)
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

        return Result.Deleted;
    }
}