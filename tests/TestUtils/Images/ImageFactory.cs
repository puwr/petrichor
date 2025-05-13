using Domain.Images;
using TestUtils.TestConstants;

namespace TestUtils.Images;

public static class ImageFactory
{
    public static Image CreateImage()
    {
        return new Image(
            path: Constants.Image.Path,
            userId: Constants.Image.UserId
        );
    }
}