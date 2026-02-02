using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Petrichor.Services.Comments.Features.CreateComment;
using Petrichor.Services.Comments.Features.GetComments;
using Petrichor.Services.Comments.Tests.TestUtilities;
using Petrichor.Services.Gallery.IntegrationMessages;
using Petrichor.Shared.Pagination;
using Petrichor.TestUtilities;
using Petrichor.TestUtilities.Authentication;
using Wolverine;

namespace Petrichor.Services.Comments.Tests.IntegrationMessageHandlers;

[Trait("Category", "Integration")]
[Collection(ApiFactoryCollection.Name)]
public class ImageDeletedIntegrationEventHandlerTests : IDisposable
{
    private readonly ApiFactory _apiFactory;
    private readonly IServiceScope _scope;
    private readonly IMessageBus _bus;

    public ImageDeletedIntegrationEventHandlerTests(ApiFactory apiFactory)
    {
        _apiFactory = apiFactory;
        _scope = _apiFactory.Services.CreateScope();
        _bus = _scope.ServiceProvider.GetRequiredService<IMessageBus>();
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

        await client.PostAsJsonAsync("/comments", new CreateCommentRequest(
            ResourceId: testResourceId,
            Message: $"Message-{Guid.NewGuid()}"));

        await _bus.PublishAsync(new ImageDeletedIntegrationEvent(testResourceId));

        await Poller.WaitAsync(TimeSpan.FromSeconds(10), async () =>
        {
            var comments = await client
                .GetFromJsonAsync<CursorPagedResponse<GetCommentsResponse>>($"/comments?resourceId={testResourceId}");
            comments.Should().NotBeNull();

            return comments.Items.Count == 0;
        });
    }
}