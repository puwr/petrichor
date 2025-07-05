using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using API.Tests.TestCommon;
using FluentAssertions;
using Infrastructure.Persistence;
using TestUtilities.Images;

namespace API.Tests.Controllers.ImagesController;

[Trait("Category", "Integration")]
[Collection(ApiFactoryCollection.Name)]
public class UploadImageControllerTests
{
    private readonly ApiFactory _apiFactory;
    private readonly HttpClient _client;

    public UploadImageControllerTests(ApiFactory apiFactory)
    {
        _apiFactory = apiFactory;
        _client = _apiFactory.HttpClient;
        _apiFactory.ResetDatabase();
    }

    [Fact]
    public async Task UploadImage_Returns201Created()
    {
        await using var imageStream = TestImages.GetImageStream("test-image.jpg");

        var formData = new MultipartFormDataContent
        {
            { new StreamContent(imageStream), "ImageFile", "test.jpg" }
        };

        var response = await _client.PostAsync("api/v1/images", formData);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var imageId = await response.Content.ReadFromJsonAsync<Guid>();
        imageId.Should().NotBeEmpty();
        response.Headers.Location.Should().Be($"/images/{imageId}");

        var dbContext = _apiFactory.GetDbContext();
        var image = await dbContext.Images.FindAsync(imageId);
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
        using (_apiFactory.AsAnonymous())
        {
            var response = await _client.PostAsync("api/v1/images", null);

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}
