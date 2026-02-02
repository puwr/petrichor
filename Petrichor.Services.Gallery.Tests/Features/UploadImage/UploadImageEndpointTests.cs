using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Petrichor.Services.Gallery.Tests.TestUtilities;
using Petrichor.TestUtilities.Authentication;

namespace Petrichor.Services.Gallery.Tests.Features.UploadImage;

[Trait("Category", "Integration")]
[Collection(ApiFactoryCollection.Name)]
public class UploadImageEndpointTests(ApiFactory apiFactory)
{
    [Fact]
    public async Task UploadImage_ReturnsCreated()
    {
        using var client = apiFactory.CreateClient();
        client.SetFakeClaims();

        var response = await client.UploadTestImageAsync();
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var uploadedImageId = await response.Content.ReadFromJsonAsync<Guid>();
        uploadedImageId.Should().NotBeEmpty();
        response.Headers.Location.Should().Be($"/images/{uploadedImageId}");
    }

    [Fact]
    public async Task UploadImage_WithInvalidImage_ReturnsBadRequest()
    {
        using var client = apiFactory.CreateClient();
        client.SetFakeClaims();

        await using var emptyStream = new MemoryStream();
        var formData = new MultipartFormDataContent
        {
            { new StreamContent(emptyStream), "ImageFile", "test.jpg" }
        };
        var response = await client.PostAsync("/images", formData);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problemDetails.Should().NotBeNull();
    }

    [Fact]
    public async Task UploadImage_WhenAnonymous_ReturnsUnauthorized()
    {
        using var client = apiFactory.CreateClient();

        var response = await client.PostAsync("/images", null);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problemDetails.Should().NotBeNull();
    }
}