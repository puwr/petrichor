using Petrichor.Shared.Events;

namespace Petrichor.Modules.Gallery.IntegrationMessages;

public class ImageDeletedIntegrationEvent(Guid imageId) : IntegrationEvent
{
    public Guid ImageId { get; init; } = imageId;
};
