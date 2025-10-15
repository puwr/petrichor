using Petrichor.Shared.DomainEvents;

namespace Petrichor.Services.Gallery.Common.Domain.Images.Events;

public class ImageDeletedDomainEvent(string imagePath, string thumbnailPath) : DomainEvent
{
    public string ImagePath { get; init; } = imagePath;
    public string ThumbnailPath { get; init; } = thumbnailPath;
};