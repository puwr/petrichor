using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Http;
using Petrichor.Modules.Shared.Application.Common.Interfaces.Services.Storage;
using Petrichor.Modules.Gallery.Application.Common.Interfaces;
using Petrichor.Modules.Gallery.Application.Common.Interfaces.Services;
using Petrichor.Modules.Gallery.Domain.Images;
using Petrichor.Modules.Gallery.Domain.Images.ValueObjects;

namespace Petrichor.Modules.Gallery.Application.Images.Commands.UploadImage;

public class UploadImageCommandHandler(
    IGalleryDbContext dbContext,
    IFileStorage fileStorage,
    IThumbnailGenerator thumbnailGenerator,
    IImageMetadataProvider imageMetadataProvider
) : IRequestHandler<UploadImageCommand, ErrorOr<Guid>>
{
    public async Task<ErrorOr<Guid>> Handle(
        UploadImageCommand command,
        CancellationToken cancellationToken)
    {
        var (originalImage, thumbnail) = await ProcessImageAsync(command.ImageFile);

        var image = new Image(originalImage, thumbnail, command.UploaderId);

        dbContext.Images.Add(image);
        await dbContext.SaveChangesAsync(cancellationToken);

        return image.Id;
    }

    private async Task<(OriginalImage, Thumbnail)> ProcessImageAsync(IFormFile imageFile)
    {
        var imageExtension = Path.GetExtension(imageFile.FileName);

        await using var imageStream = imageFile.OpenReadStream();

        var imagePath = await fileStorage.SaveFileAsync(
            imageStream,
            imageExtension,
            StorageFolders.Uploads);

        var (imageWidth, imageHeight) = await imageMetadataProvider
            .GetDimensionsAsync(imageStream);

        var thumbnailStream = await thumbnailGenerator
            .CreateThumbnailAsync(imageStream);

        var thumbnailPath = await fileStorage.SaveFileAsync(
            thumbnailStream,
            ".jpg",
            StorageFolders.Thumbnails);

        var (thumbnailWidth, thumbnailHeight) = await imageMetadataProvider
            .GetDimensionsAsync(thumbnailStream);

        var originalImage = new OriginalImage(imagePath, imageWidth, imageHeight);
        var thumbnail = new Thumbnail(thumbnailPath, thumbnailWidth, thumbnailHeight);

        return (originalImage, thumbnail);
    }
}