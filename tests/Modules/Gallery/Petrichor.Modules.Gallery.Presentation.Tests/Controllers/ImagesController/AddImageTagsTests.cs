using System.Net;
using System.Net.Http.Json;
using Petrichor.Modules.Gallery.Contracts.Images;
using TestUtilities.Authentication;
using TestUtilities.Images;

namespace Petrichor.Modules.Gallery.Presentation.Tests.Controllers.ImagesController;

[Trait("Category", "Integration")]
[Collection(ApiFactoryCollection.Name)]
public class AddImageTagsTests(ApiFactory apiFactory)
{
    [Fact]
    public async Task AddImageTags_WhenUploader_ReturnsOk()
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
            .PostAsJsonAsync($"/images/{uploadedImageId}/tags", new AddTagsRequest([tag]));
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var getImageResponse = await client.GetAsync($"/images/{uploadedImageId}");
        getImageResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var image = await getImageResponse.Content.ReadFromJsonAsync<ImageResponse>();
        image.Should().NotBeNull();
        image.Tags.Select(t => t.Name).Should().Contain(tag);
    }

    [Fact]
    public async Task AddImageTags_WhenAdmin_ReturnsOk()
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
            .PostAsJsonAsync($"/images/{uploadedImageId}/tags", new AddTagsRequest([tag]));
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task AddImageTags_WhenNotUploaderOrAdmin_ReturnsForbidden()
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
            .PostAsJsonAsync($"/images/{uploadedImageId}/tags", new AddTagsRequest([tag]));
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task AddImageTags_WithInvalidData_ReturnsBadRequest()
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
            .PostAsJsonAsync($"/images/{uploadedImageId}/tags", new AddTagsRequest([" "]));
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}