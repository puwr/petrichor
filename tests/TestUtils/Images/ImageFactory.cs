using Domain.Images;
using TestUtils.TestConstants;

namespace TestUtils.Images;

public static class ImageFactory
{
    public static Image CreateImage()
    {
        return new Image(
            imagePath: Constants.Image.ImagePath,
            thumbnailPath: Constants.Image.ThumbnailPath,
            userId: Constants.Image.UserId
        );
    }
}