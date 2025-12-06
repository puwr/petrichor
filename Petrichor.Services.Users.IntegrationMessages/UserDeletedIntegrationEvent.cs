using MemoryPack;

namespace Petrichor.Services.Users.IntegrationMessages;

[MemoryPackable]
public partial record UserDeletedIntegrationEvent(Guid UserId, bool DeleteUploadedImages);