using Application.Common.Interfaces.Services.Images;
using Infrastructure.Common.Extensions;
using SixLabors.ImageSharp;

namespace Infrastructure.Services.Images;

public class ImageMetadataProvider : IImageMetadataProvider
{
    public async Task<(int width, int height)> GetDimensionsAsync(Stream imageStream)
    {
        imageStream.Reset();

        using Image image = await Image.LoadAsync(imageStream);

        return (image.Width, image.Height);
    }
}