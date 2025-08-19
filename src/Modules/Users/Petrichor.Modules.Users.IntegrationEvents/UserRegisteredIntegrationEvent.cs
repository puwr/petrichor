using Petrichor.Shared.Application.Common.Events;

namespace Petrichor.Modules.Users.IntegrationEvents;

public class UserRegisteredIntegrationEvent(Guid userId, string userName) : IntegrationEvent
{
    public Guid UserId { get; init; } = userId;
    public string UserName { get; init; } = userName;
}