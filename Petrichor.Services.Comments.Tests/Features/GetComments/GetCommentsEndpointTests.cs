using System.Net;
using System.Net.Http.Json;
using Petrichor.Services.Comments.Features.CreateComment;
using Petrichor.Services.Comments.Features.GetComments;
using Petrichor.Services.Comments.Tests.TestUtilities;
using Petrichor.Shared.Pagination;
using Petrichor.TestUtilities.Authentication;

namespace Petrichor.Services.Comments.Tests.Features.GetComments;

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

        await client.PostAsJsonAsync("/comments", new CreateCommentRequest(
            ResourceId: testResourceId,
            Message: $"Message-{Guid.NewGuid()}"));

        var response = await client.GetAsync($"/comments?resourceId={testResourceId}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var comments = await response.Content
            .ReadFromJsonAsync<CursorPagedResponse<GetCommentsResponse>>();
        comments.Should().NotBeNull();
        comments.Items.Count.Should().BeGreaterThan(0);
    }
}