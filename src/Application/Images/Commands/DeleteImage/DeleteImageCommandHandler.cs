using Application.Common.Interfaces;
using ErrorOr;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.Images.Commands.DeleteImage;

public class DeleteImageCommandHandler(
    IPetrichorDbContext dbContext)
    : IRequestHandler<DeleteImageCommand, ErrorOr<Deleted>>
{
    public async ValueTask<ErrorOr<Deleted>> Handle(
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