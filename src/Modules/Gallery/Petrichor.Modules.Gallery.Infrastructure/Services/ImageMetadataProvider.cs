using Petrichor.Modules.Shared.Infrastructure.Common.Extensions;
using Petrichor.Modules.Gallery.Application.Common.Interfaces.Services;
using SixLabors.ImageSharp;

namespace Petrichor.Modules.Gallery.Infrastructure.Services;

public class ImageMetadataProvider : IImageMetadataProvider
{
    public async Task<(int width, int height)> GetDimensionsAsync(Stream imageStream)
    {
        imageStream.Reset();

        using Image image = await Image.LoadAsync(imageStream);

        return (image.Width, image.Height);
    }
}