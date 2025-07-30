using ErrorOr;

namespace Petrichor.Modules.Gallery.Application.Images.Commands.UploadImage;

public static class UploadImageCommandErrors
{
    public static readonly Error UserIdClaimIsMissingOrInvalid = Error.Failure(
        code: "UploadImage_UserIdClaimIsMissingOrInvalid",
        description: "User id claim is missing or invalid."
    );
}
