using Domain.Images;
using TestUtils.TestConstants;

namespace TestUtils.Images;

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