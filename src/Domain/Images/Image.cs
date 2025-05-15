using Domain.Tags;
using ErrorOr;

namespace Domain.Images;

public class Image
{
    public Guid Id { get; private set; }
    public string ImagePath { get; private set; } = null!;
    public string ThumbnailPath { get; private set; } = null!;
    public Guid UserId { get; private set; }
    public string? Description { get; private set; }
    public List<Tag> Tags { get; private set; } = [];
    public DateTime CreatedDateTime { get; private set; }

    public Image(string imagePath, string thumbnailPath, Guid userId)
    {
        Id = Guid.NewGuid();
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

    public void RemoveTag(Guid tagId)
    {
        var tagToRemove = Tags.Where(t => t.Id == tagId).FirstOrDefault();

        Tags.Remove(tagToRemove!);
    }

    private Image() { }
}