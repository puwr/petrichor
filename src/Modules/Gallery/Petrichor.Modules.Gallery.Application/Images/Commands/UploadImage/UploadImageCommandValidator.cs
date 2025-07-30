using FluentValidation;
using Microsoft.AspNetCore.Http;
using Petrichor.Modules.Gallery.Application.Common;

namespace Petrichor.Modules.Gallery.Application.Images.Commands.UploadImage;

public class UploadImageCommandValidator : AbstractValidator<UploadImageCommand>
{
    public const int MaxFileSizeMB = 25;
    public const int MaxFileSizeBytes = MaxFileSizeMB * 1024 * 1024;
    public static readonly Dictionary<string, byte[]> ImageSignatures = new()
    {
        [".jpg"] = [0xFF, 0xD8, 0xFF],
        [".jpeg"] = [0xFF, 0xD8, 0xFF],
        [".png"] = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A],
        [".webp"] = "RIFF"u8.ToArray(),
        [".gif"] = "GIF89a"u8.ToArray()
    };

    public UploadImageCommandValidator()
    {
        RuleFor(c => c.ImageFile)
            .Cascade(CascadeMode.Stop)
            .NotNull().WithMessage(GalleryValidationMessages.Image.Required)
            .Must(BeWithinSizeLimit).WithMessage(GalleryValidationMessages.Image.SizeLimit)
            .Must(HaveValidExtension)
                .WithMessage(GalleryValidationMessages.Image.SupportedFormats)
            .Must(HaveValidSignature).WithMessage(GalleryValidationMessages.Image.Corrupted);
    }

    private static bool BeWithinSizeLimit(IFormFile file) => file.Length <= MaxFileSizeBytes;
    private static bool HaveValidExtension(IFormFile file) =>
        ImageSignatures.ContainsKey(Path.GetExtension(file.FileName).ToLowerInvariant());

    private static bool HaveValidSignature(IFormFile file)
    {
        try
        {
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (ImageSignatures.TryGetValue(extension, out var signature))
            {
                using var stream = file.OpenReadStream();
                var header = new byte[signature.Length];
                int bytesRead = stream.Read(header, 0, header.Length);

                return bytesRead == header.Length && header.SequenceEqual(signature);
            }

            return false;
        }
        catch
        {
            return false;
        }

    }
}
