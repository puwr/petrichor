using Newtonsoft.Json;

namespace Petrichor.Shared.Infrastructure.Serialization;

public static class SerializerSettings
{
    public static readonly JsonSerializerSettings Events = new()
    {
        TypeNameHandling = TypeNameHandling.All,
        MetadataPropertyHandling = MetadataPropertyHandling.ReadAhead
    };
}