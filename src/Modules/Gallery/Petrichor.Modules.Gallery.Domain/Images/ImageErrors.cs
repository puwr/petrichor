using ErrorOr;

namespace Petrichor.Modules.Gallery.Domain.Images;

public static class ImageErrors
{
    public static readonly Error MaxTagsExceeded = Error.Validation(
        code: "Image.MaxTagsExceeded",
        description: $"Too many tags. Maximum is {ImageConstants.MaxTagsPerImage} per image."
    );
}
