using System.Net;
using System.Net.Http.Json;
using Minio;
using Minio.DataModel.Args;
using Petrichor.Modules.Gallery.Infrastructure.Persistence;
using TestUtilities.Images;

namespace Petrichor.Modules.Gallery.Presentation.Tests.Controllers.ImagesController;

[Trait("Category", "Integration")]
[Collection(ApiFactoryCollection.Name)]
public class UploadImageControllerTests
{
    private readonly ApiFactory _apiFactory;
    private readonly HttpClient _anonymousClient;
    private readonly HttpClient _authenticatedClient;
    private readonly IMinioClient _minioClient;
    private readonly GalleryDbContext _dbContext;

    public UploadImageControllerTests(ApiFactory apiFactory)
    {
        _apiFactory = apiFactory;
        _anonymousClient = _apiFactory.AnonymousClient;
        _authenticatedClient = _apiFactory.AuthenticatedClient;

        _minioClient = _apiFactory.MinioClient;

        _dbContext = _apiFactory.GalleryDbContext;
    }

    [Fact]
    public async Task UploadImage_Returns201Created()
    {
        await using var imageStream = TestImages.GetImageStream("test-image.jpg");

        var formData = new MultipartFormDataContent
        {
            { new StreamContent(imageStream), "ImageFile", "test.jpg" }
        };

        var response = await _authenticatedClient.PostAsync("api/v1/images", formData);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var imageId = await response.Content.ReadFromJsonAsync<Guid>();
        imageId.Should().NotBeEmpty();
        response.Headers.Location.Should().Be($"/images/{imageId}");

        var image = await _dbContext.Images.FindAsync(imageId);
        image.Should().NotBeNull();

        var imagePath = image.OriginalImage.Path.Split("/", StringSplitOptions.RemoveEmptyEntries);
        var thumbnailPath = image.Thumbnail.Path.Split("/", StringSplitOptions.RemoveEmptyEntries);

        await FluentActions.Invoking(async () =>
        {
            await _minioClient
                .StatObjectAsync(new StatObjectArgs()
                    .WithBucket(imagePath[0])
                    .WithObject(imagePath[1]));
        }).Should().NotThrowAsync();

        await FluentActions.Invoking(async () =>
        {
            await _minioClient
                .StatObjectAsync(new StatObjectArgs()
                    .WithBucket(thumbnailPath[0])
                    .WithObject(thumbnailPath[1]));
        }).Should().NotThrowAsync();
    }

    [Fact]
    public async Task UploadImage_WhenAnonymous_Returns401Unauthorized()
    {
            var response = await _anonymousClient.PostAsync("api/v1/images", null);

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}