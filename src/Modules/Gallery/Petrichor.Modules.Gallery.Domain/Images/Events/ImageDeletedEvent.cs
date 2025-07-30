using Petrichor.Shared.Domain.Common;

namespace Petrichor.Modules.Gallery.Domain.Images.Events;

public record ImageDeletedEvent(string ImagePath, string ThumbnnailPath) : IDomainEvent;