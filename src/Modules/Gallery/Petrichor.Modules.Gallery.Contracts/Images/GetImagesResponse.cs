namespace Petrichor.Modules.Gallery.Contracts.Images;

public record GetImagesResponse(
    Guid Id,
    string ThumbnailUrl,
    int ThumbnailWidth,
    int ThumbnailHeight);