using ErrorOr;
using MediatR;
using Petrichor.Services.Gallery.Common.Domain.Images;
using Petrichor.Services.Gallery.Common.Domain.Images.ValueObjects;
using Petrichor.Services.Gallery.Common.Persistence;
using Petrichor.Services.Gallery.Common.Services;
using Petrichor.Services.Gallery.Common.Storage;
using Petrichor.Shared.Services.Storage;
using ZiggyCreatures.Caching.Fusion;

namespace Petrichor.Services.Gallery.Features.UploadImage;

public class UploadImageCommandHandler(
    GalleryDbContext dbContext,
    IFileStorage fileStorage,
    IThumbnailGenerator thumbnailGenerator,
    IImageMetadataProvider imageMetadataProvider,
    IFusionCache cache
) : IRequestHandler<UploadImageCommand, ErrorOr<Guid>>
{
    public async Task<ErrorOr<Guid>> Handle(
        UploadImageCommand command,
        CancellationToken cancellationToken)
    {
        var (originalImage, thumbnail) = await ProcessImageAsync(command.ImageFile);

        var image = Image.Create(originalImage, thumbnail, command.UploaderId);

        dbContext.Images.Add(image);
        await dbContext.SaveChangesAsync(cancellationToken);

        await cache.RemoveByTagAsync("images", token: cancellationToken);

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