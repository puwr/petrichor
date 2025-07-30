using Petrichor.Modules.Gallery.Domain.Images;
using TestUtilities.TestConstants;

namespace TestUtilities.Images;

public static class ImageFactory
{
    public static Image CreateImage()
    {
        return new Image(
            originalImage: new(
                Constants.Image.OriginalImage.Path,
                Constants.Image.OriginalImage.Width,
                Constants.Image.OriginalImage.Height),
            thumbnail: new(
                Constants.Image.Thumbnail.Path,
                Constants.Image.Thumbnail.Width,
                Constants.Image.Thumbnail.Height),
            uploaderId: Constants.Image.UploaderId
        );
    }

    public static List<Image> CreateImages(int count = 1)
    {
        return Enumerable.Range(1, count)
                .Select(_ => CreateImage())
                .ToList();
    }
}