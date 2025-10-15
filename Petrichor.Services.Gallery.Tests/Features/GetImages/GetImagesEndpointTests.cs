using System.Net;
using System.Net.Http.Json;
using Petrichor.Services.Gallery.Features.AddImageTags;
using Petrichor.Services.Gallery.Features.GetImage;
using Petrichor.Services.Gallery.Tests.TestUtilities;
using Petrichor.Shared.Pagination;
using Petrichor.TestUtilities.Authentication;

namespace Petrichor.Services.Gallery.Tests.Features.GetImages;

[Trait("Category", "Integration")]
[Collection(ApiFactoryCollection.Name)]
public class GetImagesEndpointTests(ApiFactory apiFactory)
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

        var images = await response.Content.ReadFromJsonAsync<PagedResponse<GetImageResponse>>();
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
            .PostAsJsonAsync($"/images/{uploadedImageId}/tags", new AddImageTagsRequest([tag]));
        addImageTagResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var response = await client.GetAsync($"/images?tags={tag}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var images = await response.Content.ReadFromJsonAsync<PagedResponse<GetImageResponse>>();
        images.Should().NotBeNull();
        images.Count.Should().Be(1);
    }
}