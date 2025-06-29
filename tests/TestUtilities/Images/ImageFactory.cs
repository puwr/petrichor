using Domain.Images;
using TestUtilities.TestConstants;

namespace TestUtilities.Images;

public static class ImageFactory
{
    public static Image CreateImage()
    {
        return new Image(
            originalImage: Constants.Image.OriginalImage,
            thumbnail: Constants.Image.Thumbnail,
            uploaderId: Constants.Image.UploaderId
        );
    }
}