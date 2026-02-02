using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Petrichor.Services.Comments.Features.CreateComment;
using Petrichor.Services.Comments.Features.GetComments;
using Petrichor.Services.Comments.Tests.TestUtilities;
using Petrichor.Shared.Pagination;
using Petrichor.TestUtilities.Authentication;

namespace Petrichor.Services.Comments.Tests.Features.DeleteComment;

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
        var createdCommentId = await createCommentResponse.Content.ReadFromJsonAsync<Guid>();

        var response = await client.DeleteAsync($"/comments/{createdCommentId}");
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var comments = await client
            .GetFromJsonAsync<CursorPagedResponse<GetCommentsResponse>>($"/comments?resourceId={testResourceId}");
        comments.Should().NotBeNull();
        comments.Items.Count.Should().Be(0);
    }

    [Fact]
    public async Task DeleteComment_WhenAdmin_ReturnsNoContent()
    {
        using var client = apiFactory.CreateClient();
        client.SetFakeClaims();

        var testResourceId = Guid.NewGuid();

        var createCommentResponse = await client.PostAsJsonAsync("/comments", new CreateCommentRequest(
            ResourceId: testResourceId,
            Message: $"Message-{Guid.NewGuid()}"));
        var createdCommentId = await createCommentResponse.Content.ReadFromJsonAsync<Guid>();

        using var adminClient = apiFactory.CreateClient();
        adminClient.SetFakeClaims(role: "Admin");
        var response = await adminClient.DeleteAsync($"/comments/{createdCommentId}");
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteComment_WhenNotAuthorOrAdmin_ReturnsForbidden()
    {
        using var client = apiFactory.CreateClient();
        client.SetFakeClaims();

        var createCommentResponse = await client.PostAsJsonAsync("/comments", new CreateCommentRequest(
            ResourceId: Guid.NewGuid(),
            Message: $"Message-{Guid.NewGuid()}"));
        var createdCommentId = await createCommentResponse.Content.ReadFromJsonAsync<Guid>();

        using var otherClient = apiFactory.CreateClient();
        otherClient.SetFakeClaims();
        var response = await otherClient.DeleteAsync($"/comments/{createdCommentId}");
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);

        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problemDetails.Should().NotBeNull();
    }
}