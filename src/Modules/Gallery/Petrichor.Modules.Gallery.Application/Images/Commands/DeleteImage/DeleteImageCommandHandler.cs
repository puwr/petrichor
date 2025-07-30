using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Petrichor.Modules.Gallery.Application.Common.Interfaces;

namespace Petrichor.Modules.Gallery.Application.Images.Commands.DeleteImage;

public class DeleteImageCommandHandler(
    IGalleryDbContext dbContext)
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
            return Error.NotFound(description: "Image not found.");
        }

        image.DeleteImage();

        dbContext.Images.Remove(image);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Deleted;
    }
}