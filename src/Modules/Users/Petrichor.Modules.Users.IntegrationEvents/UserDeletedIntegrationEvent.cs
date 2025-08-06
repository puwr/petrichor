using Petrichor.Shared.Application.Common.Events;

namespace Petrichor.Modules.Users.IntegrationEvents;

public class UserDeletedIntegrationEvent(Guid userId, bool deleteUploadedImages) : IntegrationEvent
{
    public Guid UserId { get; init; } = userId;
    public bool DeleteUploadedImages { get; init; } = deleteUploadedImages;
}