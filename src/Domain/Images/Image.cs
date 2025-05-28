using Domain.Common;
using Domain.Images.Events;
using Domain.Images.ValueObjects;
using Domain.Tags;
using ErrorOr;

namespace Domain.Images;

public sealed class Image : Entity
{
    private readonly List<Tag> _tags = [];

    public OriginalImage OriginalImage { get; private set; }
    public Thumbnail Thumbnail { get; private set; }
    public Guid UploaderId { get; private set; }
    public IReadOnlyCollection<Tag> Tags => _tags.AsReadOnly();
    public DateTime UploadedDateTime { get; init; } = DateTime.UtcNow;

    public Image(OriginalImage originalImage, Thumbnail thumbnail, Guid uploaderId) : base(Guid.NewGuid())
    {
        OriginalImage = originalImage;
        Thumbnail = thumbnail;
        UploaderId = uploaderId;
    }

    public void AddTags(List<Tag> tags)
    {
        var tagsToAdd = tags.Except(_tags).ToList();

        _tags.AddRange(tagsToAdd);
    }

    public ErrorOr<Deleted> RemoveTag(Guid tagId)
    {
        var tagToRemove = _tags.FirstOrDefault(t => t.Id == tagId);

        if (tagToRemove is null)
        {
            return Error.NotFound("Tag not found.");
        }

        _tags.Remove(tagToRemove);

        return Result.Deleted;
    }

    public void DeleteImage()
    {
        _domainEvents.Add(new ImageDeletedEvent(OriginalImage.Path, Thumbnail.Path));
    }

    private Image() { }
}