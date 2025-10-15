using Petrichor.Shared.Events;

namespace Petrichor.Services.Users.IntegrationMessages;

public class UserDeletedIntegrationEvent(Guid userId, bool deleteUploadedImages) : IntegrationEvent
{
    public Guid UserId { get; init; } = userId;
    public bool DeleteUploadedImages { get; init; } = deleteUploadedImages;
}