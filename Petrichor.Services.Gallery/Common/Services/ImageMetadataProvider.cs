using NetVips;
using Petrichor.Shared.Extensions;

namespace Petrichor.Services.Gallery.Common.Services;

public class ImageMetadataProvider
{
    public (int width, int height) GetDimensions(Stream imageStream)
    {
        imageStream.Reset();

        using var image = Image.NewFromStream(
            imageStream,
            access: Enums.Access.Sequential);

        return (image.Width, image.Height);
    }
}