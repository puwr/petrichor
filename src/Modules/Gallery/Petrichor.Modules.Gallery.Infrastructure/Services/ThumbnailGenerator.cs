using Petrichor.Modules.Shared.Infrastructure.Common.Extensions;
using Petrichor.Modules.Gallery.Application.Common.Interfaces.Services;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace Petrichor.Modules.Gallery.Infrastructure.Services;

public class ThumbnailGenerator : IThumbnailGenerator
{
    public async Task<Stream> CreateThumbnailAsync(Stream imageStream)
    {
        imageStream.Reset();

        using Image image = await Image.LoadAsync(imageStream);

        image.Mutate(i => i.Resize(
            width: 300,
            height: 0,
            KnownResamplers.Lanczos3));

        var thumbnailStream = new MemoryStream();
        await image.SaveAsJpegAsync(thumbnailStream);

        return thumbnailStream;
    }
}