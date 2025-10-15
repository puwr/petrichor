using System.Net;
using System.Net.Http.Json;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Petrichor.Services.Comments.Common.Persistence;
using Petrichor.Services.Comments.Features.CreateComment;
using Petrichor.Services.Comments.Features.GetComments;
using Petrichor.Services.Comments.Tests.TestUtilities;
using Petrichor.Services.Users.IntegrationMessages;
using Petrichor.Shared.Pagination;
using Petrichor.TestUtilities;
using Petrichor.TestUtilities.Authentication;

namespace Petrichor.Services.Comments.Tests.IntegrationMessageHandlers;

[Trait("Category", "Integration")]
[Collection(ApiFactoryCollection.Name)]
public class UserDeletedIntegrationEventHandlerTests : IDisposable
{
    private readonly ApiFactory _apiFactory;
    private readonly IServiceScope _scope;
    private readonly CommentsDbContext _dbContext;
    private readonly IBus _bus;

    public UserDeletedIntegrationEventHandlerTests(ApiFactory apiFactory)
    {
        _apiFactory = apiFactory;
        _scope = _apiFactory.Services.CreateScope();
        _dbContext = _scope.ServiceProvider.GetRequiredService<CommentsDbContext>();
        _bus = _scope.ServiceProvider.GetRequiredService<IBus>();
    }

    public void Dispose()
    {
        _scope?.Dispose();
    }

    [Fact]
    public async Task Handle_DeletesUsersSnapshotAndComments()
    {
        var testUserId = Guid.NewGuid();
        var userRegisteredEvent = new UserRegisteredIntegrationEvent(testUserId, $"UserName-{testUserId}");
        await _bus.Publish(userRegisteredEvent);

        await Poller.WaitAsync(TimeSpan.FromSeconds(10), async () =>
        {
            var userSnapshot = await _dbContext.UserSnapshots
                .AsNoTracking()
                .FirstOrDefaultAsync(us => us.UserId == testUserId);

            return userSnapshot is not null;
        });

        using var client = _apiFactory.CreateClient();
        client.SetFakeClaims(userId: testUserId);

        var testResourceId = Guid.NewGuid();
        var request = new CreateCommentRequest(
            ResourceId: testResourceId,
            Message: $"Message-{Guid.NewGuid()}");
        var createCommentResponse = await client.PostAsJsonAsync("/comments", request);
        createCommentResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var userDeletedEvent = new UserDeletedIntegrationEvent(testUserId, false);
        await _bus.Publish(userDeletedEvent);

        await Poller.WaitAsync(TimeSpan.FromSeconds(10), async () =>
        {
            var userSnapshot = await _dbContext.UserSnapshots
                .AsNoTracking()
                .FirstOrDefaultAsync(us => us.UserId == testUserId);

            return userSnapshot is null;
        });

        var getComments = await client.GetAsync($"/comments?resourceId={testResourceId}");
        getComments.StatusCode.Should().Be(HttpStatusCode.OK);
        var comments = await getComments.Content.ReadFromJsonAsync<CursorPagedResponse<CommentResponse>>();
        comments.Should().NotBeNull();
        comments.Items.Count.Should().Be(0);
    }
}