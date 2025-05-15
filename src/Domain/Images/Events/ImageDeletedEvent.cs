using Domain.Common;

namespace Domain.Images.Events;

public record ImageDeletedEvent(string ImagePath, string ThumbnnailPath) : IDomainEvent;