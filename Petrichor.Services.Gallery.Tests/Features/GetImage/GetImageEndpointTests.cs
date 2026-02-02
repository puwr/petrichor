using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
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

        var uploadImageResponse = await client.UploadTestImageAsync();
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

        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problemDetails.Should().NotBeNull();
    }
}