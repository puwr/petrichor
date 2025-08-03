using Newtonsoft.Json;
using Petrichor.Shared.Domain.Common;

namespace Petrichor.Shared.Infrastructure.Serialization;

public static class DomainEventsSerializer
{
    private static readonly JsonSerializerSettings Settings = new()
    {
        TypeNameHandling = TypeNameHandling.All,
        MetadataPropertyHandling = MetadataPropertyHandling.ReadAhead
    };

    public static string Serialize(IDomainEvent domainEvent)
        => JsonConvert.SerializeObject(domainEvent, Settings);

    public static IDomainEvent Deserialize(string domainEvent)
        => JsonConvert.DeserializeObject<IDomainEvent>(domainEvent, Settings)!;
}
