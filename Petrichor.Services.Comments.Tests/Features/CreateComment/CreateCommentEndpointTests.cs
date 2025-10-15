using System.Net;
using System.Net.Http.Json;
using Petrichor.Services.Comments.Features.CreateComment;
using Petrichor.Services.Comments.Tests.TestUtilities;
using Petrichor.TestUtilities.Authentication;

namespace Petrichor.Services.Comments.Tests.Features.CreateComment;

[Trait("Category", "Integration")]
[Collection(ApiFactoryCollection.Name)]
public class CreateCommentEndpointTests(ApiFactory apiFactory)
{
    [Fact]
    public async Task CreateComment_ReturnsCreated()
    {
        using var client = apiFactory.CreateClient();
        client.SetFakeClaims();

        var request = new CreateCommentRequest(
            ResourceId: Guid.NewGuid(),
            Message: $"Message-{Guid.NewGuid()}");

        var response = await client.PostAsJsonAsync("/comments", request);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task CreateComment_WithInvalidData_ReturnsBadRequest()
    {
        using var client = apiFactory.CreateClient();
        client.SetFakeClaims();

        var request = new CreateCommentRequest(
            ResourceId: Guid.NewGuid(),
            Message: string.Empty);

        var response = await client.PostAsJsonAsync("/comments", request);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateComment_WhenAnonymous_ReturnsUnauthorized()
    {
        using var client = apiFactory.CreateClient();

        var response = await client.PostAsync("/comments", null);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}