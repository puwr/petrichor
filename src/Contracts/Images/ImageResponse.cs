using Contracts.Tags;

namespace Contracts.Images;

public record ImageResponse(
    Guid Id,
    string Url,
    int Width,
    int Height,
    Guid UploaderId,
    List<TagResponse> Tags,
    DateTime UploadedAt);