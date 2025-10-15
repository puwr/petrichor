namespace Petrichor.Services.Gallery.Features.GetImages;

public record GetImagesResponse(
    Guid Id,
    string ThumbnailUrl,
    int ThumbnailWidth,
    int ThumbnailHeight);