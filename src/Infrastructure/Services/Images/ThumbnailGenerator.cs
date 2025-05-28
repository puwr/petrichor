using Application.Common.Interfaces.Services.Images;
using Infrastructure.Common.Extensions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace Infrastructure.Services.Images;

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