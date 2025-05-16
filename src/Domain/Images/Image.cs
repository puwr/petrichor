using Domain.Common;
using Domain.Images.Events;
using Domain.Tags;
using ErrorOr;

namespace Domain.Images;

public class Image : Entity
{
    public string ImagePath { get; private set; } = null!;
    public string ThumbnailPath { get; private set; } = null!;
    public Guid UserId { get; private set; }
    public List<Tag> Tags { get; private set; } = [];
    public DateTime CreatedDateTime { get; private set; }

    public Image(string imagePath, string thumbnailPath, Guid userId) : base(Guid.NewGuid())
    {
        ImagePath = imagePath;
        ThumbnailPath = thumbnailPath;
        UserId = userId;
        CreatedDateTime = DateTime.UtcNow;
    }

    public ErrorOr<Success> AddTag(Tag tag)
    {
        if (Tags.Contains(tag))
        {
            return ImageErrors.TagAlreadyAdded;
        }

        Tags.Add(tag);
        return Result.Success;
    }

    public ErrorOr<Deleted> RemoveTag(Guid tagId)
    {
        var tagToRemove = Tags.Where(t => t.Id == tagId).FirstOrDefault();

        if (tagToRemove is null)
        {
            return Error.NotFound("Tag not found");
        }

        Tags.Remove(tagToRemove);

        return Result.Deleted;
    }

    public void DeleteImage()
    {
        _domainEvents.Add(new ImageDeletedEvent(ImagePath, ThumbnailPath));
    }

    private Image() { }
}