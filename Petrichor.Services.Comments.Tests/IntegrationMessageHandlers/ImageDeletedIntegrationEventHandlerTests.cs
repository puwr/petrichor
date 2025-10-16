using System.Net;
using System.Net.Http.Json;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Petrichor.Services.Comments.Features.CreateComment;
using Petrichor.Services.Comments.Features.GetComments;
using Petrichor.Services.Comments.Tests.TestUtilities;
using Petrichor.Services.Gallery.IntegrationMessages;
using Petrichor.Shared.Pagination;
using Petrichor.TestUtilities;
using Petrichor.TestUtilities.Authentication;

namespace Petrichor.Services.Comments.Tests.IntegrationMessageHandlers;

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
                .ReadFromJsonAsync<CursorPagedResponse<GetCommentsResponse>>();

            return comments!.Items.Count == 0;
        });
    }
}