using System.Net.Http.Json;
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
using Wolverine;

namespace Petrichor.Services.Comments.Tests.IntegrationMessageHandlers;

[Trait("Category", "Integration")]
[Collection(ApiFactoryCollection.Name)]
public class UserDeletedIntegrationEventHandlerTests : IDisposable
{
    private readonly ApiFactory _apiFactory;
    private readonly IServiceScope _scope;
    private readonly CommentsDbContext _dbContext;
    private readonly IMessageBus _bus;

    public UserDeletedIntegrationEventHandlerTests(ApiFactory apiFactory)
    {
        _apiFactory = apiFactory;
        _scope = _apiFactory.Services.CreateScope();
        _dbContext = _scope.ServiceProvider.GetRequiredService<CommentsDbContext>();
        _bus = _scope.ServiceProvider.GetRequiredService<IMessageBus>();
    }

    public void Dispose()
    {
        _scope?.Dispose();
    }

    [Fact]
    public async Task Handle_DeletesUserSnapshotAndComments()
    {
        var testUserId = Guid.NewGuid();

        await _bus.PublishAsync(new UserRegisteredIntegrationEvent(testUserId, $"UserName-{testUserId}"));

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

        await client.PostAsJsonAsync("/comments", new CreateCommentRequest(
            ResourceId: testResourceId,
            Message: $"Message-{Guid.NewGuid()}"));

        await _bus.PublishAsync(new UserDeletedIntegrationEvent(testUserId, false));

        await Poller.WaitAsync(TimeSpan.FromSeconds(10), async () =>
        {
            var userSnapshot = await _dbContext.UserSnapshots
                .AsNoTracking()
                .FirstOrDefaultAsync(us => us.UserId == testUserId);

            return userSnapshot is null;
        });

        var comments = await client
            .GetFromJsonAsync<CursorPagedResponse<GetCommentsResponse>>($"/comments?resourceId={testResourceId}");
        comments.Should().NotBeNull();
        comments.Items.Count.Should().Be(0);
    }
}