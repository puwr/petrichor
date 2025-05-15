namespace TestUtils.TestConstants;

public static partial class Constants
{
    public static class Image
    {
        public const string ImagePath = $"/uploads/image.jpg";
        public const string ThumbnailPath = $"/thumbs/image.jpg";
        public static readonly Guid UserId = Guid.NewGuid();
    }
}