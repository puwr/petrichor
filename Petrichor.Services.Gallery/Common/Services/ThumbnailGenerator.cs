using Petrichor.Shared.Extensions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace Petrichor.Services.Gallery.Common.Services;

public class ThumbnailGenerator : IThumbnailGenerator
{
    public async Task<Stream> CreateThumbnailAsync(
        Stream imageStream,
        CancellationToken cancellationToken = default)
    {
        imageStream.Reset();

        using Image image = await Image.LoadAsync(imageStream, cancellationToken);

        image.Mutate(i => i.Resize(
            width: 300,
            height: 0,
            KnownResamplers.Lanczos3));

        var thumbnailStream = new MemoryStream();
        await image.SaveAsJpegAsync(thumbnailStream, cancellationToken);

        return thumbnailStream;
    }
}