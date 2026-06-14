using NetVips;
using Petrichor.Shared.Extensions;

namespace Petrichor.Services.Gallery.Common.Services;

public class ThumbnailGenerator
{
    public Stream CreateThumbnail(Stream imageStream)
    {
        imageStream.Reset();

        using var image = Image.ThumbnailStream(imageStream, width: 300, size: Enums.Size.Down);

        var thumbnailStream = new MemoryStream();
        image.JpegsaveStream(thumbnailStream, q: 85);

        return thumbnailStream;
    }
}