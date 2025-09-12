using System.Net;
using System.Net.Http.Json;
using Petrichor.Modules.Gallery.Contracts.Images;
using TestUtilities.Authentication;
using TestUtilities.Images;

namespace Petrichor.Modules.Gallery.Presentation.Tests.Controllers.ImagesController;

[Trait("Category", "Integration")]
[Collection(ApiFactoryCollection.Name)]
public class DeleteImageTagTests(ApiFactory apiFactory)
{
    [Fact]
    public async Task DeleteImageTag_WhenUploader_ReturnsNoContent()
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
        var addImageTagsResponse = await client
            .PostAsJsonAsync($"/images/{uploadedImageId}/tags", new AddTagsRequest([tag]));
        addImageTagsResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var getImageResponse = await client.GetAsync($"/images/{uploadedImageId}");
        getImageResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var image = await getImageResponse.Content.ReadFromJsonAsync<ImageResponse>();
        image.Should().NotBeNull();

        var response = await client
            .DeleteAsync($"/images/{uploadedImageId}/tags/{image.Tags.FirstOrDefault()!.Id}");
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getImageWithDeletedTagResponse = await client.GetAsync($"/images/{uploadedImageId}");
        getImageWithDeletedTagResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var imageWithDeletedTag = await getImageWithDeletedTagResponse.Content.ReadFromJsonAsync<ImageResponse>();
        imageWithDeletedTag.Should().NotBeNull();
        imageWithDeletedTag.Tags.Should().BeEmpty();
    }

    [Fact]
    public async Task DeleteImageTag_WhenAdmin_ReturnsNoContent()
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
        var addImageTagsResponse = await client
            .PostAsJsonAsync($"/images/{uploadedImageId}/tags", new AddTagsRequest([tag]));
        addImageTagsResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var getImageResponse = await client.GetAsync($"/images/{uploadedImageId}");
        getImageResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var image = await getImageResponse.Content.ReadFromJsonAsync<ImageResponse>();
        image.Should().NotBeNull();

        using var adminClient = apiFactory.CreateClient();
        adminClient.SetFakeClaims(role: "Admin");

        var response = await adminClient
            .DeleteAsync($"/images/{uploadedImageId}/tags/{image.Tags.FirstOrDefault()!.Id}");
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteImageTag_WhenNotUploaderOrAdmin_ReturnsForbidden()
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
        var addImageTagsResponse = await client
            .PostAsJsonAsync($"/images/{uploadedImageId}/tags", new AddTagsRequest([tag]));
        addImageTagsResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var getImageResponse = await client.GetAsync($"/images/{uploadedImageId}");
        getImageResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var image = await getImageResponse.Content.ReadFromJsonAsync<ImageResponse>();
        image.Should().NotBeNull();

        using var otherClient = apiFactory.CreateClient();
        otherClient.SetFakeClaims();

        var response = await otherClient
            .DeleteAsync($"/images/{uploadedImageId}/tags/{image.Tags.FirstOrDefault()!.Id}");
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}