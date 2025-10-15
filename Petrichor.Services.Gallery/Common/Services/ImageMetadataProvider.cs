using Petrichor.Shared.Extensions;
using SixLabors.ImageSharp;

namespace Petrichor.Services.Gallery.Common.Services;

public class ImageMetadataProvider : IImageMetadataProvider
{
    public async Task<(int width, int height)> GetDimensionsAsync(Stream imageStream)
    {
        imageStream.Reset();

        using Image image = await Image.LoadAsync(imageStream);

        return (image.Width, image.Height);
    }
}