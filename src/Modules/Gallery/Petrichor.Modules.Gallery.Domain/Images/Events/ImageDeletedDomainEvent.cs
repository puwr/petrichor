using Petrichor.Shared.Domain.Common;

namespace Petrichor.Modules.Gallery.Domain.Images.Events;

public class ImageDeletedDomainEvent(string imagePath, string thumbnailPath) : DomainEvent
{
    public string ImagePath { get; init; } = imagePath;
    public string ThumbnailPath { get; init; } = thumbnailPath;
};