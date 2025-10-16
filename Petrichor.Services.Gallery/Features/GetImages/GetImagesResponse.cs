using Petrichor.Services.Gallery.Common.Domain.Images;

namespace Petrichor.Services.Gallery.Features.GetImages;

public record GetImagesResponse
{
    public Guid Id { get; init; }
    public string ThumbnailUrl { get; init; }
    public int ThumbnailWidth { get; init; }
    public int ThumbnailHeight { get; init; }

    public static GetImagesResponse From(Image image)
    {
        return new GetImagesResponse
        {
            Id = image.Id,
            ThumbnailUrl = image.Thumbnail.Path,
            ThumbnailWidth = image.Thumbnail.Width,
            ThumbnailHeight = image.Thumbnail.Height
        };
    }
}