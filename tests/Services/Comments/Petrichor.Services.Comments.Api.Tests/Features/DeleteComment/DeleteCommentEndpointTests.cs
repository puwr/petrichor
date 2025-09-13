using System.Net;
using System.Net.Http.Json;
using Petrichor.Services.Comments.Api.Features.CreateComment;
using Petrichor.Services.Comments.Api.Features.GetComments;
using Petrichor.Shared.Pagination;
using TestUtilities.Authentication;

namespace Petrichor.Services.Comments.Api.Tests.Features.DeleteComment;

[Trait("Category", "Integration")]
[Collection(ApiFactoryCollection.Name)]
public class DeleteCommentEndpointTests(ApiFactory apiFactory)
{
    [Fact]
    public async Task DeleteComment_WhenAuthor_ReturnsNoContent()
    {
        using var client = apiFactory.CreateClient();
        client.SetFakeClaims();

        var testResourceId = Guid.NewGuid();

        var request = new CreateCommentRequest(
            ResourceId: testResourceId,
            Message: $"Message-{Guid.NewGuid()}");
        var createCommentResponse = await client.PostAsJsonAsync("/comments", request);
        createCommentResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var commentId = await createCommentResponse.Content.ReadFromJsonAsync<Guid>();

        var response = await client.DeleteAsync($"/comments/{commentId}");
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getCommentsResponse = await client.GetAsync($"/comments?resourceId={testResourceId}");
        getCommentsResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var comments = await getCommentsResponse.Content.ReadFromJsonAsync<CursorPagedResponse<CommentResponse>>();
        comments.Should().NotBeNull();
        comments.Items.Count.Should().Be(0);
    }

    [Fact]
    public async Task DeleteComment_WhenAdmin_ReturnsNoContent()
    {
        using var client = apiFactory.CreateClient();
        client.SetFakeClaims();

        var testResourceId = Guid.NewGuid();

        var request = new CreateCommentRequest(
            ResourceId: testResourceId,
            Message: $"Message-{Guid.NewGuid()}");
        var createCommentResponse = await client.PostAsJsonAsync("/comments", request);
        createCommentResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var commentId = await createCommentResponse.Content.ReadFromJsonAsync<Guid>();

        using var adminClient = apiFactory.CreateClient();
        adminClient.SetFakeClaims(role: "Admin");
        var response = await adminClient.DeleteAsync($"/comments/{commentId}");
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteComment_WhenNotAuthorOrAdmin_ReturnsForbidden()
    {
        using var client = apiFactory.CreateClient();
        client.SetFakeClaims();

        var testResourceId = Guid.NewGuid();

        var request = new CreateCommentRequest(
            ResourceId: testResourceId,
            Message: $"Message-{Guid.NewGuid()}");
        var createCommentResponse = await client.PostAsJsonAsync("/comments", request);
        createCommentResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var commentId = await createCommentResponse.Content.ReadFromJsonAsync<Guid>();

        using var otherClient = apiFactory.CreateClient();
        otherClient.SetFakeClaims();
        var response = await otherClient.DeleteAsync($"/comments/{commentId}");
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}