using Petrichor.Services.Gallery.Common.Domain.Images;

namespace Petrichor.Services.Gallery.Features.GetImage;

public record GetImageResponse
{
    public Guid Id { get; init; }
    public string Url { get; init; }
    public int Width { get; init; }
    public int Height { get; init; }
    public Guid UploaderId { get; init; }
    public List<TagResponse> Tags { get; init; }
    public DateTime UploadedAt { get; init; }

    public static GetImageResponse From(Image image)
    {
        return new GetImageResponse
        {
            Id = image.Id,
            Url = image.OriginalImage.Path,
            Width = image.OriginalImage.Width,
            Height = image.OriginalImage.Height,
            UploaderId = image.UploaderId,
            Tags = [.. image.Tags.Select(tag => new TagResponse(tag.Id, tag.Name))],
            UploadedAt = image.UploadedDateTime
        };
    }
}

public record TagResponse(Guid Id, string Name);