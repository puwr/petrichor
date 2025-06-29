using Domain.Images.ValueObjects;

namespace TestUtilities.TestConstants;

public static partial class Constants
{
    public static class Image
    {
        public static readonly OriginalImage OriginalImage = new(
            Path: "/uploads/original.jpg",
            Width: 2000,
            Height: 1500
        );

        public static readonly Thumbnail Thumbnail = new(
            Path: "/thumbs/thumb.jpg",
            Width: 300,
            Height: 200
        );

        public static readonly Guid UploaderId = Guid.NewGuid();
    }
}