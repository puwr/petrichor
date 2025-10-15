using System.Net;
using System.Net.Http.Json;
using Petrichor.Services.Gallery.Features.GetImage;
using Petrichor.Services.Gallery.Tests.TestUtilities;
using Petrichor.TestUtilities.Authentication;

namespace Petrichor.Services.Gallery.Tests.Features.GetImage;

[Trait("Category", "Integration")]
[Collection(ApiFactoryCollection.Name)]
public class GetImageEndpointTests(ApiFactory apiFactory)
{
    [Fact]
    public async Task GetImage_ReturnsOk()
    {
        using var client = apiFactory.CreateClient();
        client.SetFakeClaims();

        await using var imageStream = TestImages.GetImageStream("test-image.jpg");
        var formData = new MultipartFormDataContent
        {
            { new StreamContent(imageStream), "ImageFile", "test.jpg" }
        };
        var uploadImageResponse = await client.PostAsync("/images", formData);
        uploadImageResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var uploadedImageId = await uploadImageResponse.Content.ReadFromJsonAsync<Guid>();

        var response = await client.GetAsync($"/images/{uploadedImageId}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var image = await response.Content.ReadFromJsonAsync<GetImageResponse>();
        image.Should().NotBeNull();
    }

    [Fact]
    public async Task GetImage_WithNonExistentId_ReturnsNotFound()
    {
        using var client = apiFactory.CreateClient();

        var response = await client.GetAsync($"/images/{Guid.NewGuid()}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}