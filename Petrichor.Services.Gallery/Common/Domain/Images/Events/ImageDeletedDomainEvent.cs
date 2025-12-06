using MemoryPack;

namespace Petrichor.Services.Gallery.Common.Domain.Images.Events;

[MemoryPackable]
public partial record ImageDeletedDomainEvent(string ImagePath, string ThumbnailPath);