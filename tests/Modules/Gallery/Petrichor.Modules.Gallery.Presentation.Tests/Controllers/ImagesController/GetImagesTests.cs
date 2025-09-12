using System.Net;
using System.Net.Http.Json;
using Petrichor.Modules.Gallery.Contracts.Images;
using Petrichor.Shared.Pagination;
using TestUtilities.Authentication;
using TestUtilities.Images;

namespace Petrichor.Modules.Gallery.Presentation.Tests.Controllers.ImagesController;

[Trait("Category", "Integration")]
[Collection(ApiFactoryCollection.Name)]
public class GetImagesTests(ApiFactory apiFactory)
{
    [Fact]
    public async Task GetImages_ReturnsOk()
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

        var response = await client.GetAsync("/images");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var images = await response.Content.ReadFromJsonAsync<PagedResponse<ImageResponse>>();
        images.Should().NotBeNull();
        images.Count.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GetImages_WithTagsQuery_ReturnsOkWithMatchingImages()
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
        var addImageTagResponse = await client
            .PostAsJsonAsync($"/images/{uploadedImageId}/tags", new AddTagsRequest([tag]));
        addImageTagResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var response = await client.GetAsync($"/images?tags={tag}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var images = await response.Content.ReadFromJsonAsync<PagedResponse<ImageResponse>>();
        images.Should().NotBeNull();
        images.Count.Should().Be(1);
    }
}