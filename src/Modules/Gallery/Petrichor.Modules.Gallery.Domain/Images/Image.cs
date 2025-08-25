using ErrorOr;
using Petrichor.Modules.Gallery.Domain.Images.ValueObjects;
using Petrichor.Modules.Gallery.Domain.Tags;
namespace Petrichor.Modules.Gallery.Domain.Images;

public sealed class Image
{
    private readonly List<Tag> _tags = [];
    public Guid Id { get; private set; }
    public OriginalImage OriginalImage { get; private set; }
    public Thumbnail Thumbnail { get; private set; }
    public Guid UploaderId { get; private set; }
    public IReadOnlyCollection<Tag> Tags => _tags.AsReadOnly();
    public DateTime UploadedDateTime { get; init; } = DateTime.UtcNow;

    public Image(OriginalImage originalImage, Thumbnail thumbnail, Guid uploaderId)
    {
        Id = Guid.CreateVersion7();
        OriginalImage = originalImage;
        Thumbnail = thumbnail;
        UploaderId = uploaderId;
    }

    public ErrorOr<Success> AddTags(List<Tag> tags)
    {
        var tagsToAdd = tags.Except(_tags).ToList();

        var potentialTotal = _tags.Count + tagsToAdd.Count;

        if (potentialTotal > ImageConstants.MaxTagsPerImage)
        {
            return ImageErrors.MaxTagsExceeded;
        }

        _tags.AddRange(tagsToAdd);

        return Result.Success;
    }

    public ErrorOr<Deleted> RemoveTag(Guid tagId)
    {
        var tagToRemove = _tags.FirstOrDefault(t => t.Id == tagId);

        if (tagToRemove is null)
        {
            return ImageErrors.TagNotAssociated;
        }

        _tags.Remove(tagToRemove);

        return Result.Deleted;
    }

    private Image() { }
}