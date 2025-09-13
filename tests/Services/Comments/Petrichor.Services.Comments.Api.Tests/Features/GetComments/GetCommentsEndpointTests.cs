using System.Net;
using System.Net.Http.Json;
using Petrichor.Services.Comments.Api.Features.CreateComment;
using Petrichor.Services.Comments.Api.Features.GetComments;
using Petrichor.Shared.Pagination;
using TestUtilities.Authentication;

namespace Petrichor.Services.Comments.Api.Tests.Features.GetComments;

[Trait("Category", "Integration")]
[Collection(ApiFactoryCollection.Name)]
public class GetCommentsEndpointTests(ApiFactory apiFactory)
{
    [Fact]
    public async Task GetComments_ReturnsOk()
    {
        using var client = apiFactory.CreateClient();
        client.SetFakeClaims();

        var testResourceId = Guid.NewGuid();

        var request = new CreateCommentRequest(
            ResourceId: testResourceId,
            Message: $"Message-{Guid.NewGuid()}");

        var createCommentResponse = await client.PostAsJsonAsync("/comments", request);
        createCommentResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var response = await client.GetAsync($"/comments?resourceId={testResourceId}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var comments = await response.Content.ReadFromJsonAsync<CursorPagedResponse<CommentResponse>>();
        comments.Should().NotBeNull();

        comments.Items.Count.Should().BeGreaterThan(0);
    }
}
