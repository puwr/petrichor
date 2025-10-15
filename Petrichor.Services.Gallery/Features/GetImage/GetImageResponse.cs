namespace Petrichor.Services.Gallery.Features.GetImage;

public record GetImageResponse(
    Guid Id,
    string Url,
    int Width,
    int Height,
    Guid UploaderId,
    List<TagResponse> Tags,
    DateTime UploadedAt);

public record TagResponse(Guid Id, string Name);