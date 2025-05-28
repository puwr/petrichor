using Contracts.Images;
using Domain.Images;

namespace Application.Common.Mappings;

public static class ImageMappingExtensions
{
    public static ImageResponse ToResponse(this Image image)
    {
        return new ImageResponse(
            Id: image.Id,
            Url: image.OriginalImage.Path,
            Width: image.OriginalImage.Width,
            Height: image.OriginalImage.Height,
            UploaderId: image.UploaderId,
            Tags: [.. image.Tags.Select(tag => tag.ToResponse())],
            UploadedAt: image.UploadedDateTime);
    }

    public static ListImagesResponse ToListResponse(this Image image)
    {
        return new ListImagesResponse(
            Id: image.Id,
            ThumbnailUrl: image.Thumbnail.Path,
            ThumbnailWidth: image.Thumbnail.Width,
            ThumbnailHeight: image.Thumbnail.Height);
    }
}