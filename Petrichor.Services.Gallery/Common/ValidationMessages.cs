using Petrichor.Services.Gallery.Features.UploadImage;

namespace Petrichor.Services.Gallery.Common;

public static class ValidationMessages
{
    public static class Image
    {
        public static string SizeLimit
            => $"Max file size is {UploadImageCommandValidator.MaxFileSizeMB}MB.";
        public static string SupportedFormats
            => $"Supported formats: {string.Join(", ",
                UploadImageCommandValidator.ImageSignatures.Keys.OrderBy(f => f))}.";
        public const string Corrupted = "File is corrupted.";
    }
}
