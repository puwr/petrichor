using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Petrichor.Services.Comments.Features.CreateComment;
using Petrichor.Services.Comments.Features.GetComments;
using Petrichor.Services.Comments.Tests.TestUtilities;
using Petrichor.Shared.Pagination;
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

        var testResourceId = Guid.NewGuid();
        var testMessage = $"Message-{Guid.NewGuid()}";

        var request = new CreateCommentRequest(
            ResourceId: testResourceId,
            Message: testMessage);

        var response = await client.PostAsJsonAsync("/comments", request);
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var comments = await client
            .GetFromJsonAsync<CursorPagedResponse<GetCommentsResponse>>($"/comments?resourceId={testResourceId}");
        comments.Should().NotBeNull();
        comments.Items[0].Message.Should().Be(testMessage);
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

        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problemDetails.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateComment_WhenAnonymous_ReturnsUnauthorized()
    {
        using var client = apiFactory.CreateClient();

        var response = await client.PostAsync("/comments", null);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problemDetails.Should().NotBeNull();
    }
}