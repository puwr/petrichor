using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
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

        var uploadImageResponse = await client.UploadTestImageAsync();
        var uploadedImageId = await uploadImageResponse.Content.ReadFromJsonAsync<Guid>();

        var tag = $"tag-{Guid.NewGuid()}";
        var response = await client
            .PatchAsJsonAsync($"/images/{uploadedImageId}/tags", new UpdateImageTagsRequest([tag]));
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var image = await client.GetFromJsonAsync<GetImageResponse>($"/images/{uploadedImageId}");
        image.Should().NotBeNull();
        image.Tags.Should().BeEquivalentTo([tag]);
    }

    [Fact]
    public async Task UpdateImageTags_WhenAdmin_ReturnsNoContent()
    {
        using var client = apiFactory.CreateClient();
        client.SetFakeClaims();

        var uploadImageResponse = await client.UploadTestImageAsync();
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

        var problemDetails = response.Content.ReadFromJsonAsync<ProblemDetails>();
        problemDetails.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdateImageTags_WhenNotUploaderOrAdmin_ReturnsForbidden()
    {
        using var client = apiFactory.CreateClient();
        client.SetFakeClaims();

        var uploadImageResponse = await client.UploadTestImageAsync();
        var uploadedImageId = await uploadImageResponse.Content.ReadFromJsonAsync<Guid>();

        using var otherClient = apiFactory.CreateClient();
        otherClient.SetFakeClaims();

        var tag = $"tag-{Guid.NewGuid()}";
        var response = await otherClient
            .PatchAsJsonAsync($"/images/{uploadedImageId}/tags", new UpdateImageTagsRequest([tag]));
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);

        var problemDetails = response.Content.ReadFromJsonAsync<ProblemDetails>();
        problemDetails.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdateImageTags_WithInvalidData_ReturnsBadRequest()
    {
        using var client = apiFactory.CreateClient();
        client.SetFakeClaims();

        var uploadImageResponse = await client.UploadTestImageAsync();
        var uploadedImageId = await uploadImageResponse.Content.ReadFromJsonAsync<Guid>();

        var response = await client
            .PatchAsJsonAsync($"/images/{uploadedImageId}/tags", new UpdateImageTagsRequest([" "]));
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var problemDetails = response.Content.ReadFromJsonAsync<ProblemDetails>();
        problemDetails.Should().NotBeNull();
    }
}