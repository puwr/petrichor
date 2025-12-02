namespace Petrichor.Services.Users.IntegrationMessages;

public record UserDeletedIntegrationEvent(Guid UserId, bool DeleteUploadedImages);