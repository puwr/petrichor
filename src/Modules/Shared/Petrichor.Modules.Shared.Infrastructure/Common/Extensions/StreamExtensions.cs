namespace Petrichor.Modules.Shared.Infrastructure.Common.Extensions;

public static class StreamExtensions
{
    public static void Reset(this Stream stream)
    {
        if (stream.CanSeek && stream.Position > 0)
        {
            stream.Position = 0;
        }
    }
}