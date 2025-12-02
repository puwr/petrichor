namespace Petrichor.Services.Gallery.Common.Domain.Images.Events;

public record ImageDeletedDomainEvent(string ImagePath, string ThumbnailPath);