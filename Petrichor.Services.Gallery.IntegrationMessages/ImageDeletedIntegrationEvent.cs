using Petrichor.Shared.Events;

namespace Petrichor.Services.Gallery.IntegrationMessages;

public class ImageDeletedIntegrationEvent(Guid imageId) : IntegrationEvent
{
    public Guid ImageId { get; init; } = imageId;
};
