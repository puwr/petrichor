using System.Security.Claims;
using Application.Common.Interfaces;
using Domain.Images;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Images.Commands.UploadImage;

public class UploadImageCommandHandler(
    IImagesRepository imagesRepository,
    IUnitOfWork unitOfWork,
    IHttpContextAccessor httpContextAccessor
) : IRequestHandler<UploadImageCommand, ErrorOr<Image>>
{
    public async Task<ErrorOr<Image>> Handle(UploadImageCommand command, CancellationToken cancellationToken)
    {
        var currentUserIdClaim = httpContextAccessor.HttpContext.User
            .FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(currentUserIdClaim, out Guid currentUserId))
        {
            return Error.Unauthorized("User identification missing.");
        }

        var image = new Image(command.ImagePath, command.ThumbnailPath, currentUserId);

        await imagesRepository.AddImageAsync(image);
        await unitOfWork.CommitChangesAsync();

        return image;
    }
}
