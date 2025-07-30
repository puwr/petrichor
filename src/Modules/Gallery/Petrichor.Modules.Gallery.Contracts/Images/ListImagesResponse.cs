namespace Petrichor.Modules.Gallery.Contracts.Images;

public record ListImagesResponse(
    Guid Id,
    string ThumbnailUrl,
    int ThumbnailWidth,
    int ThumbnailHeight);