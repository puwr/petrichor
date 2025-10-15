using Petrichor.Shared.Events;

namespace Petrichor.Services.Users.IntegrationMessages;

public class UserRegisteredIntegrationEvent(Guid userId, string userName) : IntegrationEvent
{
    public Guid UserId { get; init; } = userId;
    public string UserName { get; init; } = userName;
}