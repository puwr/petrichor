using MemoryPack;

namespace Petrichor.Services.Gallery.IntegrationMessages;

[MemoryPackable]
public partial record ImageDeletedIntegrationEvent(Guid ImageId);