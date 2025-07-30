using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ErrorOr;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Petrichor.Modules.Gallery.Application.Common;
using Petrichor.Modules.Gallery.Application.Common.Interfaces.Services;
using Petrichor.Modules.Gallery.Application.Images.Commands.UploadImage;
using Petrichor.Shared.Application.Common.Interfaces.Services.Storage;

namespace Petrichor.Modules.Gallery.Application.Tests.Images.Commands;

public class UploadImageTests : IDisposable
{
    private readonly MediatorFactory _mediatorFactory;
    private static readonly byte[] JpegSignature =
        UploadImageCommandValidator.ImageSignatures[".jpg"];

    public UploadImageTests()
    {
        var fileStorageMock = Substitute.For<IFileStorage>();
        fileStorageMock.SaveFileAsync(Arg.Any<Stream>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns("/test/test.jpg");

        _mediatorFactory = new(services =>
        {
            services.AddSingleton(fileStorageMock);
            services.AddSingleton(Substitute.For<IThumbnailGenerator>());
            services.AddSingleton(Substitute.For<IImageMetadataProvider>());
        });
    }

    [Fact]
    public async Task Handle_ReturnsImageId()
    {
        var mediator = _mediatorFactory.Create(services =>
        {
            var httpContextAccessorMock = Substitute.For<IHttpContextAccessor>();
            var user = new ClaimsPrincipal(new ClaimsIdentity(
                [
                    new Claim(JwtRegisteredClaimNames.Sub, Guid.NewGuid().ToString())
                ]));

            httpContextAccessorMock.HttpContext!.User.Returns(user);

            services.AddSingleton(httpContextAccessorMock);
        });

        var imageFile = CreateTestFile(JpegSignature, "test.jpg");

        var uploadImageResult = await mediator.Send(new UploadImageCommand(imageFile));

        uploadImageResult.IsError.Should().BeFalse();
        uploadImageResult.Value.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Handle_WhenUserIdClaimIsMissingOrInvalid_ReturnsUserIdClaimIsMissingOrInvalidError()
    {
        var mediator = _mediatorFactory.Create(services =>
        {
            var httpContextAccessorMock = Substitute.For<IHttpContextAccessor>();
            var user = new ClaimsPrincipal(new ClaimsIdentity(
                [
                    new Claim(JwtRegisteredClaimNames.Sub, "test")
                ]));

            httpContextAccessorMock.HttpContext!.User.Returns(user);

            services.AddSingleton(httpContextAccessorMock);
        });

        var imageFile = CreateTestFile(JpegSignature, "test.jpg");

        var uploadImageResult = await mediator.Send(new UploadImageCommand(imageFile));

        uploadImageResult.IsError.Should().BeTrue();
        uploadImageResult.FirstError.Should().Be(UploadImageCommandErrors.UserIdClaimIsMissingOrInvalid);
    }

    [Fact]
    public async Task Handle_WhenMaxFileSizeExceeded_ReturnsValidationError()
    {
        const int maxFileSizeBytes = UploadImageCommandValidator.MaxFileSizeBytes;

        var mediator = _mediatorFactory.Create();

        var fileContent = new byte[maxFileSizeBytes + 1];
        Buffer.BlockCopy(JpegSignature, 0, fileContent, 0, JpegSignature.Length);

        var imageFile = CreateTestFile(fileContent, "test.jpg");

        var uploadImageResult = await mediator.Send(new UploadImageCommand(imageFile));

        uploadImageResult.IsError.Should().BeTrue();
        uploadImageResult.FirstError.Type.Should().BeOneOf(ErrorType.Validation);
        uploadImageResult.FirstError.Description
            .Should().Be(GalleryValidationMessages.Image.SizeLimit);
    }

    [Fact]
    public async Task Handle_WhenFileHasInvalidExtension_ReturnsValidationError()
    {
        var mediator = _mediatorFactory.Create();

        var imageFile = CreateTestFile(JpegSignature, "test.pdf");

        var uploadImageResult = await mediator.Send(new UploadImageCommand(imageFile));

        uploadImageResult.IsError.Should().BeTrue();
        uploadImageResult.FirstError.Type.Should().BeOneOf(ErrorType.Validation);
        uploadImageResult.FirstError.Description
            .Should().Be(GalleryValidationMessages.Image.SupportedFormats);
    }

    [Fact]
    public async Task Handle_WhenFileHasInvalidSignature_ReturnsValidationError()
    {
        var mediator = _mediatorFactory.Create();

        var imageFile = CreateTestFile([], "test.jpg");

        var uploadImageResult = await mediator.Send(new UploadImageCommand(imageFile));

        uploadImageResult.IsError.Should().BeTrue();
        uploadImageResult.FirstError.Type.Should().BeOneOf(ErrorType.Validation);
        uploadImageResult.FirstError.Description.Should().Be(GalleryValidationMessages.Image.Corrupted);
    }

    private static FormFile CreateTestFile(byte[] fileBytes, string fileName)
    {
        var imageStream = new MemoryStream(fileBytes);

        var imageFile = new FormFile(
            imageStream,
            0,
            imageStream.Length,
            "ImageFile",
            fileName);

        return imageFile;
    }

    public void Dispose()
    {
        _mediatorFactory.Dispose();
    }
}
