using FluentValidation.TestHelper;
using Microsoft.AspNetCore.Http;
using Petrichor.Services.Gallery.Common;
using Petrichor.Services.Gallery.Features.UploadImage;

namespace Petrichor.Services.Gallery.Tests.Features.UploadImage;

public class UploadImageCommandValidatorTests
{
    private readonly UploadImageCommandValidator _validator = new();

    [Fact]
    public void WithValidImageFile_PassesValidation()
    {
        var file = CreateFakeImageFile("test.jpg", [0xFF, 0xD8, 0xFF]);
        var command = new UploadImageCommand(file, Guid.NewGuid());

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void WithFileOutOfSizeLimit_ReturnsValidationError()
    {
        var file = CreateFakeImageFile("big.jpg", new byte[UploadImageCommandValidator.MaxFileSizeBytes + 1]);
        var command = new UploadImageCommand(file, Guid.NewGuid());

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.ImageFile)
            .WithErrorMessage(ValidationMessages.Image.SizeLimit);
    }

    [Theory]
    [InlineData("invalid")]
    [InlineData("invalid.exe")]
    [InlineData("invalid.txt")]
    public void WithUnsupportedFileExtension_ReturnsValidationError(string fileName)
    {
        var file = CreateFakeImageFile(fileName, []);
        var command = new UploadImageCommand(file, Guid.NewGuid());

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.ImageFile)
            .WithErrorMessage(ValidationMessages.Image.SupportedFormats);
    }

    [Fact]
    public void WithCorruptedFile_ReturnsValidationError()
    {
        // PNG, but with JPG signature
        var file = CreateFakeImageFile("corrupted.png", [0xFF, 0xD8, 0xFF]);
        var command = new UploadImageCommand(file, Guid.NewGuid());

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.ImageFile)
            .WithErrorMessage(ValidationMessages.Image.Corrupted);
    }

    private static FormFile CreateFakeImageFile(string fileName, byte[] content)
    {
        var stream = new MemoryStream(content);

        return new FormFile(stream, 0, content.Length, "ImageFile", fileName);
    }
}