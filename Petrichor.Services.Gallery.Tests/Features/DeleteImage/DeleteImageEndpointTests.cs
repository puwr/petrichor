using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Petrichor.Services.Gallery.Common.Domain.Images.Events;
using Petrichor.Services.Gallery.IntegrationMessages;
using Petrichor.Services.Gallery.Tests.TestUtilities;
using Petrichor.TestUtilities;
using Petrichor.TestUtilities.Authentication;

namespace Petrichor.Services.Gallery.Tests.Features.DeleteImage;

[Trait("Category", "Integration")]
[Collection(ApiFactoryCollection.Name)]
public class DeleteImageEndpointTests(ApiFactory apiFactory)
{
    [Fact]
    public async Task DeleteImage_WhenUploader_ReturnsNoContent()
    {
        using var client = apiFactory.CreateClient();
        client.SetFakeClaims();

        var uploadImageResponse = await client.UploadTestImageAsync();
        var uploadedImageId = await uploadImageResponse.Content.ReadFromJsonAsync<Guid>();

        var (response, session) = await apiFactory.Services
            .TrackHttpCall(async () => await client.DeleteAsync($"/images/{uploadedImageId}"));

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        session.Sent.SingleMessage<ImageDeletedDomainEvent>();
        session.Sent.SingleMessage<ImageDeletedIntegrationEvent>().ImageId.Should().Be(uploadedImageId);

        var getImageResponse = await client.GetAsync($"/images/{uploadedImageId}");
        getImageResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteImage_WhenAdmin_ReturnsNoContent()
    {
        using var client = apiFactory.CreateClient();
        client.SetFakeClaims();

        var uploadImageResponse = await client.UploadTestImageAsync();
        var uploadedImageId = await uploadImageResponse.Content.ReadFromJsonAsync<Guid>();

        using var adminClient = apiFactory.CreateClient();
        adminClient.SetFakeClaims(role: "Admin");

        var response = await adminClient.DeleteAsync($"/images/{uploadedImageId}");
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteImage_WhenAnonymous_ReturnsUnauthorized()
    {
        using var client = apiFactory.CreateClient();

        var response = await client.DeleteAsync($"/images/{Guid.NewGuid()}");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problemDetails.Should().NotBeNull();
    }

    [Fact]
    public async Task DeleteImage_WhenNotUploaderOrAdmin_ReturnsForbidden()
    {
        using var client = apiFactory.CreateClient();
        client.SetFakeClaims();

        var uploadImageResponse = await client.UploadTestImageAsync();
        var uploadedImageId = await uploadImageResponse.Content.ReadFromJsonAsync<Guid>();

        using var otherClient = apiFactory.CreateClient();
        otherClient.SetFakeClaims();

        var otherResponse = await otherClient.DeleteAsync($"/images/{uploadedImageId}");
        otherResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);

        var problemDetails = await otherResponse.Content.ReadFromJsonAsync<ProblemDetails>();
        problemDetails.Should().NotBeNull();
    }
}