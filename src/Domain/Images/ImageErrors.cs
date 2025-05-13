using ErrorOr;

namespace Domain.Images;

public static class ImageErrors
{
    public static readonly Error TagAlreadyAdded = 
        Error.Conflict("Image.TagAlreadyAdded", "Tag already added to the image");
}