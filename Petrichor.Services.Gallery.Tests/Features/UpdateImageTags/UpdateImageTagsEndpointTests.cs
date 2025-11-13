using System.Net;
using System.Net.Http.Json;
using Petrichor.Services.Gallery.Features.GetImage;
using Petrichor.Services.Gallery.Features.UpdateImageTags;
using Petrichor.Services.Gallery.Tests.TestUtilities;
using Petrichor.TestUtilities.Authentication;

namespace Petrichor.Services.Gallery.Tests.Features.UpdateImageTags;

[Trait("Category", "Integration")]
[Collection(ApiFactoryCollection.Name)]
public class UpdateImageTagsEndpointTests(ApiFactory apiFactory)
{
    [Fact]
    public async Task UpdateImageTags_WhenUploader_ReturnsNoContent()
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

        var tag = $"tag-{Guid.NewGuid()}";
        var response = await client
            .PatchAsJsonAsync($"/images/{uploadedImageId}/tags", new UpdateImageTagsRequest([tag]));
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getImageResponse = await client.GetAsync($"/images/{uploadedImageId}");
        getImageResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var image = await getImageResponse.Content.ReadFromJsonAsync<GetImageResponse>();
        image.Should().NotBeNull();
        image.Tags.Should().BeEquivalentTo([tag]);
    }

    [Fact]
    public async Task UpdateImageTags_WhenAdmin_ReturnsNoContent()
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

        using var adminClient = apiFactory.CreateClient();
        adminClient.SetFakeClaims(role: "Admin");

        var tag = $"tag-{Guid.NewGuid()}";
        var response = await adminClient
            .PatchAsJsonAsync($"/images/{uploadedImageId}/tags", new UpdateImageTagsRequest([tag]));
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task UpdateImageTags_WhenAnonymous_ReturnsUnauthorized()
    {
        using var client = apiFactory.CreateClient();

        var response = await client
            .PatchAsync($"/images/{Guid.NewGuid()}/tags", null);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateImageTags_WhenNotUploaderOrAdmin_ReturnsForbidden()
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

        using var otherClient = apiFactory.CreateClient();
        otherClient.SetFakeClaims();

        var tag = $"tag-{Guid.NewGuid()}";
        var response = await otherClient
            .PatchAsJsonAsync($"/images/{uploadedImageId}/tags", new UpdateImageTagsRequest([tag]));
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpdateImageTags_WithInvalidData_ReturnsBadRequest()
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

        var response = await client
            .PatchAsJsonAsync($"/images/{uploadedImageId}/tags", new UpdateImageTagsRequest([" "]));
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}