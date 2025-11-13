using ErrorOr;
using Petrichor.Services.Gallery.Common.Domain.Images.ValueObjects;

namespace Petrichor.Services.Gallery.Common.Domain.Images;

public sealed class Image
{
    private readonly List<Tag> _tags = [];
    public Guid Id { get; private set; }
    public OriginalImage OriginalImage { get; private set; }
    public Thumbnail Thumbnail { get; private set; }
    public Guid UploaderId { get; private set; }
    public IReadOnlyCollection<Tag> Tags => _tags.AsReadOnly();
    public DateTime UploadedDateTime { get; init; } = DateTime.UtcNow;

    public static Image Create(OriginalImage originalImage, Thumbnail thumbnail, Guid uploaderId)
    {
        return new Image
        {
            Id = Guid.CreateVersion7(),
            OriginalImage = originalImage,
            Thumbnail = thumbnail,
            UploaderId = uploaderId
        };
    }

    public ErrorOr<Success> UpdateTags(List<Tag> tags)
    {
        if (tags.Count > ImageConstants.MaxTagsPerImage)
        {
            return ImageErrors.MaxTagsExceeded;
        }

        _tags.Clear();

        _tags.AddRange(tags);

        return Result.Success;
    }
}