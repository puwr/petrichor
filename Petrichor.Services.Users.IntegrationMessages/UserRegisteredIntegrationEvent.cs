namespace Petrichor.Services.Users.IntegrationMessages;

public record UserRegisteredIntegrationEvent(Guid UserId, string UserName);