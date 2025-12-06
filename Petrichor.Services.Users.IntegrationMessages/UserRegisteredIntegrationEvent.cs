using MemoryPack;

namespace Petrichor.Services.Users.IntegrationMessages;

[MemoryPackable]
public partial record UserRegisteredIntegrationEvent(Guid UserId, string UserName);