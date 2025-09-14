using System.Net;
using System.Net.Http.Json;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Petrichor.Modules.Gallery.IntegrationMessages;
using Petrichor.Services.Comments.Api.Features.CreateComment;
using Petrichor.Services.Comments.Api.Features.GetComments;
using Petrichor.Shared.Pagination;
using TestUtilities;
using TestUtilities.Authentication;

namespace Petrichor.Services.Comments.Api.Tests.IntegrationMessageHandlers;

[Trait("Category", "Integration")]
[Collection(ApiFactoryCollection.Name)]
public class ImageDeletedIntegrationEventHandlerTests : IDisposable
{
    private readonly ApiFactory _apiFactory;
    private readonly IServiceScope _scope;
    private readonly IBus _bus;

    public ImageDeletedIntegrationEventHandlerTests(ApiFactory apiFactory)
    {
        _apiFactory = apiFactory;
        _scope = _apiFactory.Services.CreateScope();
        _bus = _scope.ServiceProvider.GetRequiredService<IBus>();
    }

    public void Dispose()
    {
        _scope?.Dispose();
    }

    [Fact]
    public async Task Handle_DeletesCommentsOfDeletedImage()
    {
        using var client = _apiFactory.CreateClient();
        client.SetFakeClaims();

        var testResourceId = Guid.NewGuid();
        var request = new CreateCommentRequest(
            ResourceId: testResourceId,
            Message: $"Message-{Guid.NewGuid()}");
        var createCommentResponse = await client.PostAsJsonAsync("/comments", request);
        createCommentResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var imageDeletedEvent = new ImageDeletedIntegrationEvent(testResourceId);
        await _bus.Publish(imageDeletedEvent);

        await Poller.WaitAsync(TimeSpan.FromSeconds(10), async () =>
        {
            var getCommentsResponse = await client.GetAsync($"/comments?resourceId={testResourceId}");
            getCommentsResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var comments = await getCommentsResponse.Content
                .ReadFromJsonAsync<CursorPagedResponse<CommentResponse>>();

            return comments!.Items.Count == 0;
        });
    }
}