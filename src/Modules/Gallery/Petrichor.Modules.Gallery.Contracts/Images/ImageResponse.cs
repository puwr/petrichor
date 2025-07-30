using Petrichor.Modules.Gallery.Contracts.Tags;

namespace Petrichor.Modules.Gallery.Contracts.Images;

public record ImageResponse(
    Guid Id,
    string Url,
    int Width,
    int Height,
    Guid UploaderId,
    List<TagResponse> Tags,
    DateTime UploadedAt);