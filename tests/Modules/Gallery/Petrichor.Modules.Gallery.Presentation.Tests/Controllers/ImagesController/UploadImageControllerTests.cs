using System.Net;
using System.Net.Http.Json;
using TestUtilities.Images;

namespace Petrichor.Modules.Gallery.Presentation.Tests.Controllers.ImagesController;

[Trait("Category", "Integration")]
[Collection(ApiFactoryCollection.Name)]
public class UploadImageControllerTests
{
    private readonly ApiFactory _apiFactory;
    private readonly HttpClient _anonymousClient;
    private readonly HttpClient _authenticatedClient;

    public UploadImageControllerTests(ApiFactory apiFactory)
    {
        _apiFactory = apiFactory;
        _anonymousClient = _apiFactory.AnonymousClient;
        _authenticatedClient = _apiFactory.AuthenticatedClient;
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

        var image = await _apiFactory.GalleryDbContext.Images.FindAsync(imageId);
        image.Should().NotBeNull();

        var sanitizedImagePath = Path.Combine(image.OriginalImage.Path.Split("/"));
        var sanitizedThumbnailPath = Path.Combine(image.Thumbnail.Path.Split("/"));

        File.Exists(Path.Combine(_apiFactory.TestDataFolder, sanitizedImagePath))
            .Should()
            .BeTrue();

        File.Exists(Path.Combine(_apiFactory.TestDataFolder, sanitizedThumbnailPath))
            .Should()
            .BeTrue();
    }

    [Fact]
    public async Task UploadImage_WhenAnonymous_Returns401Unauthorized()
    {
            var response = await _anonymousClient.PostAsync("api/v1/images", null);

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
