using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Petrichor.Modules.Gallery.Application.Common.Interfaces;

namespace Petrichor.Modules.Gallery.Application.Images.Commands.DeleteImageTag;

public class DeleteImageTagCommandHandler(
    IGalleryDbContext dbContext)
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
        {
            return Error.NotFound("Image not found.");
        }

        var removeTagResult = image.RemoveTag(command.TagId);

        if (removeTagResult.IsError)
        {
            return removeTagResult.Errors;
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Deleted;
    }
}